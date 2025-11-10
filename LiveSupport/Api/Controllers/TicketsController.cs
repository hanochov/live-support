using Api.Dtos.Tickets;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    // GET /api/tickets?status=0&priority=2&search=login
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var items = await _service.ListAsync(status, priority, search, ct);
        return Ok(items);
    }

    // GET /api/tickets/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        try
        {
            var ticket = await _service.GetByIdAsync(id, ct);
            return Ok(ticket);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /api/tickets
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto.Title, dto.Description, dto.CustomerEmail, dto.Priority, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/tickets/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto.Title, dto.Description, dto.Priority, ct);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH /api/tickets/5/status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateStatusAsync(id, dto.Status, ct);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE /api/tickets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
