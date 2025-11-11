using Application.Interfaces;
using Api.Dtos.Agents;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController(IAgentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Agent>>> List([FromQuery] bool? isActive)
        => Ok(await service.ListAsync(isActive));

    [HttpPost]
    public async Task<ActionResult<Agent>> Create([FromBody] CreateAgentDto dto)
    {
        var created = await service.CreateAsync(dto.Name, dto.Email);
        return CreatedAtAction(nameof(List), new { id = created.Id }, created);
    }
}
