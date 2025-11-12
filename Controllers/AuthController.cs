using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ProyexBackend.Models;
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

// --- NEW DTOs for Profile CRUD ---
public record UserProfileDto(int Id, string Username, string Email, string? FirstName, string? LastName, string Role);
public record UpdateProfileDto(string? Username, string? Email, string? FirstName, string? LastName);


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

    // ==================================================================
    // == START: Added CRUD Endpoints
    // ==================================================================

    /// <summary>
    /// [READ] Gets the profile of the currently authenticated user.
    /// </summary>
    [HttpGet("profile")]
    [Authorize] // 🔒 Requires authentication
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound("User not found."); // Should be rare if token is valid
        }

        // Return a DTO, not the full User model (to hide PasswordHash)
        var profile = new UserProfileDto(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString()
        );
        return Ok(profile);
    }

    /// <summary>
    /// [UPDATE] Updates the profile of the currently authenticated user.
    /// </summary>
    [HttpPut("profile")]
    [Authorize] // 🔒 Requires authentication
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Check for email conflicts (if email is being changed)
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already taken.");
            }
            user.Email = request.Email;
        }
        
        // Check for username conflicts (if username is being changed)
        if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username already taken.");
            }
            user.Username = request.Username;
        }

        // Update optional fields (using ?? to keep existing value if null)
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully." });
    }

    /// <summary>
    /// [DELETE] Deletes the account of the currently authenticated user.
    /// </summary>
    [HttpDelete("profile")]
    [Authorize] // 🔒 Requires authentication
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Note: Real-world scenarios require handling related data 
        // (e.g., reassigning projects, or cascading deletes configured in the DB).
        // This is a simple removal.
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User account deleted successfully." });
    }

    // ==================================================================
    // == END: Added CRUD Endpoints
    // ==================================================================


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
    
    /// <summary>
    /// Helper method to get the ID of the user from their token.
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                          ??
                          User.Claims.FirstOrDefault(c => c.Type == "sub"); // "sub" is standard for user ID

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        // This should not happen if [Authorize] is working
        throw new Exception("User ID not found in token.");
    }
}