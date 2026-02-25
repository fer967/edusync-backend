

namespace EduSync.Domain.Entities;

public class StudentProgress
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
