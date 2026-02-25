using EduSync.Domain.Entities;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson);
    Task<IEnumerable<Lesson>> GetByCourseAsync(Guid courseId);
    // 👇 NUEVOS MÉTODOS
    Task<Lesson?> GetByIdAsync(Guid id);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(Lesson lesson);
}
