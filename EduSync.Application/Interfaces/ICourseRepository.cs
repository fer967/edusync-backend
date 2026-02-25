using EduSync.Domain.Entities;

namespace EduSync.Application.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(Guid id);
    Task AddAsync(Course course);
    Task<Course?> GetByIdTrackingAsync(Guid id);
    Task UpdateAsync(Course course);
    Task<List<Course>> GetAllWithProgressAsync(Guid studentId);
    
    Task DeleteAsync(Course course);
}