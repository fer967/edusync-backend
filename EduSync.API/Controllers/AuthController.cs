using EduSync.Domain.Entities;
using EduSync.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    private readonly AppDbContext _context;

    public AuthController(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Students
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return Unauthorized();

        var validPassword = BCrypt.Net.BCrypt
            .Verify(request.Password, user.PasswordHash);

        if (!validPassword)
            return Unauthorized();

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("El username es obligatorio");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("La contraseña es obligatoria");

        if (request.Password.Length < 6)
            return BadRequest("La contraseña debe tener al menos 6 caracteres");

        var exists = await _context.Students
            .AnyAsync(u => u.Username == request.Username);

        if (exists)
            return BadRequest("El username ya existe");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var student = new Student
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Role = "Student"
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return Ok("Usuario creado correctamente");
    }
  
}

    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string Username, string Password);


//[HttpPost("register")]
//[AllowAnonymous]
//public async Task<IActionResult> Register([FromBody] RegisterRequest request)
//{
//    var exists = await _context.Students
//        .AnyAsync(u => u.Username == request.Username);

//    if (exists)
//        return BadRequest("Username already exists");

//    var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

//    var student = new Student
//    {
//        Username = request.Username,
//        PasswordHash = passwordHash,
//        Role = "Student"
//    };

//    _context.Students.Add(student);
//    await _context.SaveChangesAsync();

//    return Ok("Student created");
//}



