using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class UserGroupsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public UserGroupsController(ProyexDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserGroup>>> GetUserGroups()
    {
        return await _context.UserGroups.ToListAsync();
    }

    [HttpGet("{userId}/{groupId}")]
    public async Task<ActionResult<UserGroup>> GetUserGroup(int userId, int groupId)
    {
        var item = await _context.UserGroups.FindAsync(userId, groupId);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<UserGroup>> Create(UserGroup ug)
    {
        _context.UserGroups.Add(ug);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserGroup), new { userId = ug.UserId, groupId = ug.GroupId }, ug);
    }

    [HttpDelete("{userId}/{groupId}")]
    public async Task<IActionResult> Delete(int userId, int groupId)
    {
        var item = await _context.UserGroups.FindAsync(userId, groupId);
        if (item == null) return NotFound();

        _context.UserGroups.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
