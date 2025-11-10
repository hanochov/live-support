namespace Api.Dtos.Tickets;

public class UpdateTicketDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; } // נשמור כ-int כדי שיהיה נוח ב-Swagger
}
