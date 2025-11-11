using Application.Interfaces;
using Application.DTOs.Tickets;
using Microsoft.AspNetCore.Mvc;
using Api.Dtos.Tickets;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> List(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] string? search,
        [FromQuery] int? agentId,
        CancellationToken ct = default)
        => Ok(await service.ListAsync(status, priority, search, agentId, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDto>> Get(int id, CancellationToken ct = default)
        => Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketDto dto, CancellationToken ct = default)
    {
        var created = await service.CreateAsync(dto.Title, dto.Description, dto.CustomerEmail, dto.Priority, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketDto>> Update(int id, [FromBody] UpdateTicketDto dto, CancellationToken ct = default)
        => Ok(await service.UpdateAsync(id, dto.Title, dto.Description, dto.Priority, ct));

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TicketDto>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto, CancellationToken ct = default)
        => Ok(await service.UpdateStatusAsync(id, dto.Status, ct));

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignAgentDto dto, CancellationToken ct = default)
    {
        await service.AssignAgentAsync(id, dto.AgentId, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
