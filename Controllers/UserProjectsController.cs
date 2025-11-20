using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class UserProjectsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public UserProjectsController(ProyexDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProject>>> GetUserProjects()
    {
        return await _context.UserProjects.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToProject(UserProject up)
    {
        _context.UserProjects.Add(up);
        await _context.SaveChangesAsync();
        return Ok(up);
    }

    [HttpDelete("{userId}/{projectId}")]
    public async Task<IActionResult> RemoveUserFromProject(int userId, int projectId)
    {
        var up = await _context.UserProjects.FindAsync(userId, projectId);
        if (up == null) return NotFound();

        _context.UserProjects.Remove(up);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
