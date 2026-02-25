using System.ComponentModel.DataAnnotations.Schema;

namespace EduSync.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsCompleted { get; set; }

    public string? FilePath { get; set; }
    public string? FileName { get; set; }
}

