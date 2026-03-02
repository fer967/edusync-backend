using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonRepository _repository;
    private readonly IFileStorageService _fileStorage;

    public LessonsController(ILessonRepository repository, IFileStorageService fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

          

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
    {
        var lesson = new Lesson
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Content = dto.Content
        };

        await _repository.AddAsync(lesson);

        return Ok(lesson);
    }


    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        var lessons = await _repository.GetByCourseAsync(courseId);
        return Ok(lessons);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Lesson updatedLesson)
    {
        var lesson = await _repository.GetByIdAsync(id);

        if (lesson == null)
            return NotFound();

        lesson.Title = updatedLesson.Title;
        lesson.Content = updatedLesson.Content;

        await _repository.UpdateAsync(lesson);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var lesson = await _repository.GetByIdAsync(id);

        if (lesson == null)
            return NotFound();

        await _repository.DeleteAsync(lesson);

        return NoContent();
    }


    [HttpPost("{lessonId}/upload")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadFile(Guid lessonId, IFormFile file)
    {
        var lesson = await _repository.GetByIdAsync(lessonId);

        if (lesson == null)
            return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("Archivo inválido");

        using var stream = file.OpenReadStream();

        var (url, publicId) = await _fileStorage.UploadFileAsync(stream, file.FileName);

        lesson.FilePath = url;
        lesson.FileName = file.FileName;
        lesson.CloudinaryPublicId = publicId;

        await _repository.UpdateAsync(lesson);

        return Ok(new { lesson.FileName, lesson.FilePath });
    }

            
    [HttpDelete("{lessonId}/file")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLessonFile(Guid lessonId)
    {
        var lesson = await _repository.GetByIdAsync(lessonId);

        if (lesson == null)
            return NotFound();

        if (!string.IsNullOrEmpty(lesson.CloudinaryPublicId))
        {
            await _fileStorage.DeleteFileAsync(lesson.CloudinaryPublicId);
        }

        lesson.FileName = null;
        lesson.FilePath = null;
        lesson.CloudinaryPublicId = null;

        await _repository.UpdateAsync(lesson);

        return NoContent();
    }
  
       
}

public record CreateLessonDto(
    Guid CourseId,
    string Title,
    string Content
);