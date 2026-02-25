using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentProgressController : ControllerBase
{
    private readonly IStudentProgressRepository _repository;

    public StudentProgressController(IStudentProgressRepository repository)
    {
        _repository = repository;
    }

    // ✅ GET api/studentprogress
    [HttpGet]
    public async Task<IActionResult> GetMyProgress()
    {
        var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (studentIdClaim is null)
            return Unauthorized();

        if (!Guid.TryParse(studentIdClaim, out var studentId))
            return Unauthorized();

        var progress = await _repository.GetByStudentAsync(studentId);

        return Ok(progress);
    }


    // ✅ POST api/studentprogress/sync
    [HttpPost("sync")]
    public async Task<IActionResult> SyncProgress([FromBody] List<StudentProgressDto> incoming)
    {
        var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (studentIdClaim is null)
            return Unauthorized();

        if (!Guid.TryParse(studentIdClaim, out var studentId))
            return Unauthorized();

        foreach (var item in incoming)
        {
            var existing = await _repository
                .GetByStudentAndLessonAsync(studentId, item.LessonId);

            if (existing == null)
            {
                await _repository.AddAsync(new StudentProgress
                {
                    StudentId = studentId,
                    LessonId = item.LessonId,
                    IsCompleted = item.IsCompleted,
                    UpdatedAt = item.UpdatedAt
                });
            }

            else
            {
                existing.IsCompleted = item.IsCompleted;
                existing.UpdatedAt = item.UpdatedAt;

                await _repository.UpdateAsync(existing);
            }
        }

        await _repository.SaveChangesAsync();

        return Ok();
    }


// ✅ GET api/studentprogress/course/{courseId}
[HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (studentIdClaim is null)
            return Unauthorized();

        if (!Guid.TryParse(studentIdClaim, out var studentId))
            return Unauthorized();

        var progress = await _repository.GetByStudentAndCourseAsync(studentId, courseId);

        return Ok(progress);
    } }



    public record StudentProgressDto(
    Guid LessonId,
    bool IsCompleted,
    DateTime UpdatedAt
);
