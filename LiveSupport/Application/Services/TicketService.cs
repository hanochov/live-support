using Application.DTOs.Tickets;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TicketService(ITicketRepository repo, ITicketNotifier notifier) : ITicketService
{
    public async Task<IEnumerable<TicketDto>> ListAsync(int? status, int? priority, string? search, int? agentId, CancellationToken ct = default)
    {
        var list = await repo.GetAsync(status, priority, search, ct);
        if (agentId is not null) list = list.Where(t => t.AgentId == agentId);
        return list.Select(Map);
    }

    public async Task<TicketDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        return Map(entity);
    }

    public async Task<TicketDto> CreateAsync(string title, string? desc, string customerEmail, int priority, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(customerEmail)) throw new ArgumentException("CustomerEmail is required");

        var entity = new Ticket
        {
            Title = title.Trim(),
            Description = desc,
            CustomerEmail = customerEmail.Trim(),
            Priority = (TicketPriority)priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        await notifier.TicketCreatedAsync(entity);

        return Map(entity);
    }

    public async Task<TicketDto> UpdateAsync(int id, string title, string? desc, int priority, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");

        entity.Title = title.Trim();
        entity.Description = desc;
        entity.Priority = (TicketPriority)priority;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.SaveChangesAsync(ct);
        await notifier.TicketUpdatedAsync(entity);

        return Map(entity);
    }

    public async Task<TicketDto> UpdateStatusAsync(int id, int status, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        entity.Status = (TicketStatus)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.SaveChangesAsync(ct);
        await notifier.TicketStatusChangedAsync(entity);

        return Map(entity);
    }

    public async Task AssignAgentAsync(int ticketId, int? agentId, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(ticketId, ct) ?? throw new KeyNotFoundException();
        entity.AgentId = agentId;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.SaveChangesAsync(ct);
        await notifier.TicketUpdatedAsync(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        await repo.RemoveAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        await notifier.TicketDeletedAsync(id);
    }

    private static TicketDto Map(Ticket t) => new TicketDto
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        CustomerEmail = t.CustomerEmail,
        Priority = t.Priority.ToString(),
        Status = t.Status.ToString(),
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt,
        AgentId = t.AgentId,
        AgentName = t.Agent?.Name
    };
}
