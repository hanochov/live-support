using Domain.Entities;

namespace Application.Interfaces;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAsync(int? status, int? priority, string? search, CancellationToken ct = default);
    Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Ticket ticket, CancellationToken ct = default);
    Task RemoveAsync(Ticket ticket, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
