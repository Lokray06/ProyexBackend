using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectPermissionsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public ProjectPermissionsController(ProyexDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectPermission>>> GetPermissions()
    {
        return await _context.ProjectPermissions.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePermission(ProjectPermission perm)
    {
        _context.ProjectPermissions.Add(perm);
        await _context.SaveChangesAsync();
        return Ok(perm);
    }

    [HttpDelete("{userId}/{projectId}")]
    public async Task<IActionResult> DeletePermission(int userId, int projectId)
    {
        var perm = await _context.ProjectPermissions.FindAsync(userId, projectId);
        if (perm == null) return NotFound();

        _context.ProjectPermissions.Remove(perm);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
