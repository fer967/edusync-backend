using Microsoft.EntityFrameworkCore;
using EduSync.Domain.Entities;

namespace EduSync.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<StudentProgress> StudentProgress => Set<StudentProgress>();

    public DbSet<Student> Students { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title)
                  .IsRequired()
                  .HasMaxLength(200);
        });

        // Lesson
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.HasOne(l => l.Course)
                  .WithMany(c => c.Lessons)
                  .HasForeignKey(l => l.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(l => l.Title)
                  .IsRequired()
                  .HasMaxLength(200);
        });

        // StudentProgress
        modelBuilder.Entity<StudentProgress>(entity =>
        {
            entity.HasKey(sp => sp.Id);

            entity.HasOne(sp => sp.Lesson)
                  .WithMany()
                  .HasForeignKey(sp => sp.LessonId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}