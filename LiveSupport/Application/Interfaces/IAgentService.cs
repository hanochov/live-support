using Domain.Entities;

namespace Application.Interfaces;

public interface IAgentService
{
    Task<IEnumerable<Agent>> ListAsync(bool? isActive = null, CancellationToken ct = default);
    Task<Agent> CreateAsync(string name, string email, CancellationToken ct = default);
}
