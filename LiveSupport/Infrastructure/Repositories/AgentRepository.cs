using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AgentRepository(AppDbContext db) : IAgentRepository
{
    public Task<Agent?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Agents.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<Agent>> ListAsync(bool? isActive = null, CancellationToken ct = default)
    {
        var q = db.Agents.AsNoTracking().AsQueryable();
        if (isActive is not null) q = q.Where(a => a.IsActive == isActive);
        return await q.OrderBy(a => a.Name).ToListAsync(ct);
    }

    public Task AddAsync(Agent agent, CancellationToken ct = default)
        => db.Agents.AddAsync(agent, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
