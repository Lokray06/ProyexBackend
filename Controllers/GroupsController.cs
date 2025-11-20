using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public GroupsController(ProyexDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
    {
        return await _context.Groups.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return NotFound();
        return group;
    }

    [HttpPost]
    public async Task<ActionResult<Group>> CreateGroup(Group group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(int id, Group group)
    {
        if (id != group.Id) return BadRequest();

        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return NotFound();

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
