using Domain.Entities;

namespace Application.Interfaces;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Agent>> ListAsync(bool? isActive = null, CancellationToken ct = default);
    Task AddAsync(Agent agent, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
