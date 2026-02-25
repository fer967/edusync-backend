using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using EduSync.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Security.Claims;

namespace EduSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]


[Authorize]

public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _repository;

    private readonly AppDbContext _context;

    public CoursesController(ICourseRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var studentIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (studentIdString == null)
            return Unauthorized();

        var studentId = Guid.Parse(studentIdString);

        var courses = await _repository.GetAllWithProgressAsync(studentId);

        return Ok(courses);
    }

    

    [HttpGet("{id}")]
    
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        return Ok(course);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Course course)
    {
        if (string.IsNullOrWhiteSpace(course.Title))
            return BadRequest("Title es obligatorio");
        if (string.IsNullOrWhiteSpace(course.Description))
            return BadRequest("Description is required");

        await _repository.AddAsync(course);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Course updatedCourse)
    {
        var course = await _repository.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        course.Title = updatedCourse.Title;
        course.Description = updatedCourse.Description;

        await _repository.UpdateAsync(course);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var course = await _repository.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        await _repository.DeleteAsync(course);

        return NoContent();
    }

    [HttpGet("{courseId}/certificate")]
    [Authorize]
    public async Task<IActionResult> GenerateCertificate(Guid courseId)
    {
        var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var studentName = User.FindFirst(ClaimTypes.Name)?.Value;

        if (studentId == null)
            return Unauthorized();

        var course = await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            return NotFound();

        var progress = await _context.StudentProgress
            .Where(sp => sp.StudentId.ToString() == studentId)
            .ToListAsync();

        var totalLessons = course.Lessons.Count;
        var completedLessons = course.Lessons.Count(l =>
            progress.Any(p => p.LessonId == l.Id && p.IsCompleted));

        if (totalLessons == 0 || completedLessons != totalLessons)
            return BadRequest("Curso no completado");

        var pdf = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);

                page.Content().Column(col =>
                {
                    col.Spacing(20);

                    col.Item().Text("CERTIFICADO DE FINALIZACIÓN")
                        .FontSize(28)
                        .Bold()
                        .AlignCenter();

                    col.Item().Text($"Se certifica que")
                        .FontSize(18)
                        .AlignCenter();

                    col.Item().Text(studentName)
                        .FontSize(22)
                        .Bold()
                        .AlignCenter();

                    col.Item().Text($"ha completado satisfactoriamente el curso")
                        .FontSize(18)
                        .AlignCenter();

                    col.Item().Text(course.Title)
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    col.Item().Text($"Fecha: {DateTime.UtcNow:dd/MM/yyyy}")
                        .FontSize(14)
                        .AlignCenter();
                });
            });
        }).GeneratePdf();

        return File(pdf, "application/pdf", $"Certificado_{course.Title}.pdf");
    }

}




