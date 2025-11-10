using Api.Hubs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public class SignalRNotifier(IHubContext<SupportHub> hub) : ITicketNotifier
{
    public Task TicketCreatedAsync(Ticket ticket)
        => hub.Clients.All.SendAsync("TicketCreated", ticket);

    public Task TicketUpdatedAsync(Ticket ticket)
        => hub.Clients.All.SendAsync("TicketUpdated", ticket);

    public Task TicketStatusChangedAsync(Ticket ticket)
        => hub.Clients.All.SendAsync("TicketStatusChanged", new { ticket.Id, ticket.Status, ticket.UpdatedAt });

    public Task TicketDeletedAsync(int ticketId)
        => hub.Clients.All.SendAsync("TicketDeleted", new { Id = ticketId });
}
