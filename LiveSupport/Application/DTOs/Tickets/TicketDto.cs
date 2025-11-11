namespace Application.DTOs.Tickets;

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string CustomerEmail { get; set; } = default!;
    public string Priority { get; set; } = default!;  
    public string Status { get; set; } = default!;    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int? AgentId { get; set; }
    public string? AgentName { get; set; }
}
