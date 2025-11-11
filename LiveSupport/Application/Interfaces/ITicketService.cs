using Domain.Entities;

namespace Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> ListAsync(int? status, int? priority, string? search, int? agentId, CancellationToken ct = default);
    Task<Ticket> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Ticket> CreateAsync(string title, string? desc, string customerEmail, int priority, CancellationToken ct = default);
    Task<Ticket> UpdateAsync(int id, string title, string? desc, int priority, CancellationToken ct = default);
    Task<Ticket> UpdateStatusAsync(int id, int status, CancellationToken ct = default);
    Task AssignAgentAsync(int ticketId, int? agentId, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
