using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyexBackend.Data;
using ProyexBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class TaskTagsController : ControllerBase
{
    private readonly ProyexDBContext _context;

    public TaskTagsController(ProyexDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskTag>>> GetTaskTags()
    {
        return await _context.TaskTags.ToListAsync();
    }

    [HttpGet("{taskId}/{tagId}")]
    public async Task<ActionResult<TaskTag>> GetTaskTag(int taskId, int tagId)
    {
        var item = await _context.TaskTags.FindAsync(taskId, tagId);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<TaskTag>> Create(TaskTag tt)
    {
        _context.TaskTags.Add(tt);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTaskTag), new { taskId = tt.TaskId, tagId = tt.TagId }, tt);
    }

    [HttpDelete("{taskId}/{tagId}")]
    public async Task<IActionResult> Delete(int taskId, int tagId)
    {
        var item = await _context.TaskTags.FindAsync(taskId, tagId);
        if (item == null) return NotFound();

        _context.TaskTags.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
