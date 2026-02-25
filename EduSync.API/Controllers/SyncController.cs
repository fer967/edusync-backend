using EduSync.Application.DTOs;
using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ICourseRepository _repository;

    public SyncController(ICourseRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] SyncRequestDto request)
    {
        foreach (var dto in request.Courses)
        {
            if (dto.Id == null || dto.Id == Guid.Empty)
            {
                // Crear nuevo
                var newCourse = new Course
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(newCourse);
            }
            else
            {
                var existing = await _repository.GetByIdTrackingAsync(dto.Id.Value);

                if (existing != null)
                {
                    // Resolución básica por fecha
                    if (dto.UpdatedAt > existing.UpdatedAt)
                    {
                        existing.Title = dto.Title;
                        existing.Description = dto.Description;
                        existing.UpdatedAt = DateTime.UtcNow;

                        await _repository.UpdateAsync(existing);
                    }
                }
            }
        }

        return Ok(new { message = "Sincronización completada" });
    }
}