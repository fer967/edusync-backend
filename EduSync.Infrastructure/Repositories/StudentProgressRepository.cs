using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using EduSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduSync.Infrastructure.Repositories;

public class StudentProgressRepository : IStudentProgressRepository
{
    private readonly AppDbContext _context;

    public StudentProgressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentProgress>> GetByStudentAsync(Guid studentId)
    {
        return await _context.StudentProgress
            .Where(p => p.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<StudentProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId)
    {
        return await _context.StudentProgress
            .FirstOrDefaultAsync(p =>
                p.StudentId == studentId &&
                p.LessonId == lessonId);
    }

   

    public async Task AddAsync(StudentProgress progress)
    {
        await _context.StudentProgress.AddAsync(progress);
    }

    public Task UpdateAsync(StudentProgress progress)
    {
        _context.StudentProgress.Update(progress);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StudentProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.StudentProgress
            .Include(p => p.Lesson)
            .Where(p => p.StudentId == studentId &&
                        p.Lesson.CourseId == courseId)
            .ToListAsync();
    }
}