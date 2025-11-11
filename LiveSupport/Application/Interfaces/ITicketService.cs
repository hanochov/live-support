using Application.DTOs.Tickets;

namespace Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> ListAsync(int? status, int? priority, string? search, int? agentId, CancellationToken ct = default);
        Task<TicketDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<TicketDto> CreateAsync(string title, string? desc, string customerEmail, int priority, CancellationToken ct = default);
        Task<TicketDto> UpdateAsync(int id, string title, string? desc, int priority, CancellationToken ct = default);
        Task<TicketDto> UpdateStatusAsync(int id, int status, CancellationToken ct = default);

        Task AssignAgentAsync(int ticketId, int? agentId, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
