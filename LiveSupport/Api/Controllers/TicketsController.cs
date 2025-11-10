using Api.Dtos.Tickets;
using Infrastructure.Data;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TicketsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/tickets?status=0&priority=2&search=login
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] string? search)
    {
        var q = _db.Tickets.AsNoTracking().AsQueryable();

        if (status is not null)
            q = q.Where(t => (int)t.Status == status);

        if (priority is not null)
            q = q.Where(t => (int)t.Priority == priority);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(t => t.Title.Contains(search) || (t.Description ?? "").Contains(search));

        var list = await q.OrderByDescending(t => t.UpdatedAt).ToListAsync();
        return Ok(list);
    }

    // GET /api/tickets/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ticket>> GetById(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    // POST /api/tickets
    [HttpPost]
    public async Task<ActionResult<Ticket>> Create([FromBody] CreateTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.CustomerEmail))
            return BadRequest("Title and CustomerEmail are required.");

        var entity = new Ticket
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            CustomerEmail = dto.CustomerEmail.Trim(),
            Priority = (TicketPriority)dto.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Tickets.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    // PUT /api/tickets/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Ticket>> Update(int id, [FromBody] UpdateTicketDto dto)
    {
        var entity = await _db.Tickets.FindAsync(id);
        if (entity is null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description;
        entity.Priority = (TicketPriority)dto.Priority;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // PATCH /api/tickets/5/status
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<Ticket>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var entity = await _db.Tickets.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Status = (TicketStatus)dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE /api/tickets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Tickets.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Tickets.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
