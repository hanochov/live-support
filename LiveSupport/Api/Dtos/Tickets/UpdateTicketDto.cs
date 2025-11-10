using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Tickets;

public class UpdateTicketDto
{
    [Required, StringLength(120)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, 3)]
    public int Priority { get; set; }
}
