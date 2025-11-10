using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TicketService(ITicketRepository repo, ITicketNotifier notifier) : ITicketService
{
    public Task<IEnumerable<Ticket>> ListAsync(int? status, int? priority, string? search, CancellationToken ct = default)
        => repo.GetAsync(status, priority, search, ct);

    public async Task<Ticket> GetByIdAsync(int id, CancellationToken ct = default)
        => await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();

    public async Task<Ticket> CreateAsync(string title, string? desc, string customerEmail, int priority, CancellationToken ct = default)
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
        return entity;
    }

    public async Task<Ticket> UpdateAsync(int id, string title, string? desc, int priority, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");

        entity.Title = title.Trim();
        entity.Description = desc;
        entity.Priority = (TicketPriority)priority;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.SaveChangesAsync(ct);
        await notifier.TicketUpdatedAsync(entity);
        return entity;
    }

    public async Task<Ticket> UpdateStatusAsync(int id, int status, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        entity.Status = (TicketStatus)status;
        entity.UpdatedAt = DateTime.UtcNow;

        await repo.SaveChangesAsync(ct);
        await notifier.TicketStatusChangedAsync(entity);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        await repo.RemoveAsync(entity, ct);
        await repo.SaveChangesAsync(ct);

        await notifier.TicketDeletedAsync(id);
    }
}
