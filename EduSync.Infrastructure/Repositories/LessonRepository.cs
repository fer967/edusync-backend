using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using EduSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Lesson>> GetByCourseAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId && !l.IsDeleted)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lesson lesson)
    {
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }
}