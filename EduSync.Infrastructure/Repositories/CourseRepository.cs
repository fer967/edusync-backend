using EduSync.Application.Interfaces;
using EduSync.Domain.Entities;
using EduSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduSync.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

       
    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task<Course?> GetByIdTrackingAsync(Guid id)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task<Course?> GetByIdWithLessonsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Lessons.Where(l => !l.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Course>> GetAllWithProgressAsync(Guid studentId)
    {
        var courses = await _context.Courses
            .Include(c => c.Lessons)
            .ToListAsync();

        var progress = await _context.StudentProgress
            .Where(sp => sp.StudentId == studentId)
            .ToListAsync();

        foreach (var course in courses)
        {
            foreach (var lesson in course.Lessons)
            {
                lesson.IsCompleted = progress
                    .Any(p => p.LessonId == lesson.Id && p.IsCompleted);
            }
        }

        return courses;
    }

    //public async Task UpdateAsync(Course course)
    //{
    //    _context.Courses.Update(course);
    //    await _context.SaveChangesAsync();
    //}

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}