using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProyexBackend.Data;
using ProyexBackend.Models;
using Microsoft.EntityFrameworkCore;

// Simple DTOs (Data Transfer Objects) for login/register
public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponse(string Token, string Username, string Email);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ProyexDBContext _context;
    private readonly IConfiguration _config;

    public AuthController(ProyexDBContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
        {
            return BadRequest("Username or email already taken.");
        }

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Worker // Default role from your enum
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        // Generate JWT Token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponse(token, user.Username, user.Email));
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Add claims (user information)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // Add the user's role
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(24), // Token expires in 24 hours
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}