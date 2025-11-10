namespace Api.Dtos.Tickets;

public class CreateTicketDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public int Priority { get; set; } = 0; // 0=Low,1=Medium,2=High,3=Critical
}
