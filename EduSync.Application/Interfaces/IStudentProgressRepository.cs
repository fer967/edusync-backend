using EduSync.Domain.Entities;

namespace EduSync.Application.Interfaces;

public interface IStudentProgressRepository
{
    Task<List<StudentProgress>> GetByStudentAsync(Guid studentId);

    Task<StudentProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId);

    Task AddAsync(StudentProgress progress);

    Task UpdateAsync(StudentProgress progress);

    Task SaveChangesAsync();

    Task<IEnumerable<StudentProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
}