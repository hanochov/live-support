using Domain.Entities;

namespace Application.Interfaces;

public interface ITicketNotifier
{
    Task TicketCreatedAsync(Ticket ticket);
    Task TicketUpdatedAsync(Ticket ticket);
    Task TicketStatusChangedAsync(Ticket ticket);
    Task TicketDeletedAsync(int ticketId);
}
