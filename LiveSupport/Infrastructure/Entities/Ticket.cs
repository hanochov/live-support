namespace Infrastructure.Entities;

public enum TicketPriority { Low, Medium, High, Critical }
public enum TicketStatus { Open, InProgress, Resolved }

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public TicketPriority Priority { get; set; } = TicketPriority.Low;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
