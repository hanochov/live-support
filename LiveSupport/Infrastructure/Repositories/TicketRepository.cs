using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    public async Task<IEnumerable<Ticket>> GetAsync(int? status, int? priority, string? search, CancellationToken ct = default)
    {
        var q = db.Tickets.AsNoTracking().AsQueryable();

        if (status is not null) q = q.Where(t => (int)t.Status == status);
        if (priority is not null) q = q.Where(t => (int)t.Priority == priority);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(t => t.Title.Contains(search) || (t.Description ?? "").Contains(search));

        return await q.OrderByDescending(t => t.UpdatedAt).ToListAsync(ct);
    }

    public Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task AddAsync(Ticket ticket, CancellationToken ct = default)
        => db.Tickets.AddAsync(ticket, ct).AsTask();

    public Task RemoveAsync(Ticket ticket, CancellationToken ct = default)
    { db.Tickets.Remove(ticket); return Task.CompletedTask; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
