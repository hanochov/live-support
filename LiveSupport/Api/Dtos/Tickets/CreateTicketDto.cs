using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Tickets;

public class CreateTicketDto
{
    [Required, StringLength(120)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required, EmailAddress, StringLength(254)]
    public string CustomerEmail { get; set; } = null!;

    [Range(0, 3)] // 0=Low,1=Medium,2=High,3=Critical
    public int Priority { get; set; } = 0;
}
