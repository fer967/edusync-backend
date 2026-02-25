namespace EduSync.Domain.Entities;

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Student";

    public ICollection<StudentProgress> Progress { get; set; } = new List<StudentProgress>();
}
