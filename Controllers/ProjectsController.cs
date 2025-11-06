using Microsoft.AspNetCore.Authorization; // <-- Import authorization
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize] // <-- 🔒 ALL endpoints in this controller require a valid JWT
public class ProjectsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public ProjectsController(ProyexDBContext context)
    {
        _context = context;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        // Example: Get only projects owned by the currently logged-in user
        var userId = GetCurrentUserId();
        var projects = await _context.Projects
            .Where(p => p.OwnerId == userId)
            .ToListAsync();

        return Ok(projects);
    }

    // GET: api/projects/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return NotFound();
        }

        // Security check: Is this user allowed to see this project?
        var userId = GetCurrentUserId();
        if (project.OwnerId != userId)
        {
            // You'd add more complex logic here (check userProjects, group permissions, etc.)
            return Forbid("You do not have permission to view this project.");
        }

        return Ok(project);
    }

    // POST: api/projects
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] Project project)
    {
        // Set the owner to the logged-in user
        project.OwnerId = GetCurrentUserId();
        project.CreatedAt = DateTime.UtcNow; // Ensure dates are set

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Return a 201 Created response with the new project
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // A helper method to get the ID of the user from their token
    private int GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                          ?? User.Claims.FirstOrDefault(c => c.Type == "sub"); // "sub" is standard for user ID

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        // This should not happen if [Authorize] is working
        throw new Exception("User ID not found in token.");
    }
}