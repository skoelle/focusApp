using FocusApp.Data;
using FocusApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FocusTasksController : ControllerBase
{
    private readonly FocusContext _context;
    private readonly ILogger<FocusTasksController> _logger;

    public FocusTasksController(FocusContext context, ILogger<FocusTasksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FocusTask>>> GetTasks()
    {
        var tasks = await _context.FocusTasks
            .OrderByDescending(t => t.Order)
            .ToListAsync();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<FocusTask>> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (dto.Title.Length > 255)
            return BadRequest("Title must not exceed 255 characters");

        if (dto.Description != null && dto.Description.Length > 2000)
            return BadRequest("Description must not exceed 2000 characters");

        var maxOrder = await _context.FocusTasks.MaxAsync(t => (int?)t.Order) ?? 0;

        var task = new FocusTask
        {
            Title = dto.Title,
            Description = dto.Description,
            Order = maxOrder + 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.FocusTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.FocusTasks.FindAsync(id);
        if (task == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            if (dto.Title.Length > 255)
                return BadRequest("Title must not exceed 255 characters");
            task.Title = dto.Title;
        }

        if (dto.Description != null)
        {
            if (dto.Description.Length > 2000)
                return BadRequest("Description must not exceed 2000 characters");
            task.Description = dto.Description;
        }

        if (dto.Order.HasValue)
            task.Order = dto.Order.Value;

        task.UpdatedAt = DateTime.UtcNow;

        _context.FocusTasks.Update(task);
        await _context.SaveChangesAsync();

        return Ok(task);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderTasks([FromBody] ReorderDto dto)
    {
        if (dto.Orders == null || dto.Orders.Count == 0)
            return BadRequest("Orders are required");

        // Finde maximale Order-Nummer
        int maxOrder = dto.Orders.Max(o => o.Order);

        foreach (var order in dto.Orders)
        {
            var task = await _context.FocusTasks.FindAsync(order.Id);
            if (task != null)
            {
                // Invertiere die Order: h�chste wird niedrigste und umgekehrt
                task.Order = maxOrder - order.Order;
                task.UpdatedAt = DateTime.UtcNow;
                _context.FocusTasks.Update(task);
            }
        }

        await _context.SaveChangesAsync();

        // R�ckgabe: absteigend sortiert (neue oben)
        return Ok(await _context.FocusTasks.OrderByDescending(t => t.Order).ToListAsync());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.FocusTasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.FocusTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}

public class ReorderDto
{
    public List<OrderItem> Orders { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    public int Order { get; set; }
}
