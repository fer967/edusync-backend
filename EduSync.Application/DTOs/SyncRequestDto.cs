namespace EduSync.Application.DTOs;

public class SyncRequestDto
{
    public List<SyncCourseDto> Courses { get; set; } = new();
}
