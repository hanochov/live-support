using Application.Interfaces;
using Api.Dtos.Tickets;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> List(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] string? search,
        [FromQuery] int? agentId)
        => Ok(await service.ListAsync(status, priority, search, agentId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ticket>> Get(int id)
        => Ok(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<Ticket>> Create([FromBody] CreateTicketDto dto)
    {
        var created = await service.CreateAsync(dto.Title, dto.Description, dto.CustomerEmail, dto.Priority);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Ticket>> Update(int id, [FromBody] UpdateTicketDto dto)
        => Ok(await service.UpdateAsync(id, dto.Title, dto.Description, dto.Priority));

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<Ticket>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        => Ok(await service.UpdateStatusAsync(id, dto.Status));

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignAgentDto dto)
    {
        await service.AssignAgentAsync(id, dto.AgentId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
