using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AgentService(IAgentRepository repo) : IAgentService
{
    public Task<IEnumerable<Agent>> ListAsync(bool? isActive = null, CancellationToken ct = default)
        => repo.ListAsync(isActive, ct);

    public async Task<Agent> CreateAsync(string name, string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");

        var entity = new Agent { Name = name.Trim(), Email = email.Trim() };
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return entity;
    }
}
