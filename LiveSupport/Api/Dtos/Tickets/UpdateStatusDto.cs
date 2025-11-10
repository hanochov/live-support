using System.ComponentModel.DataAnnotations;

namespace Api.Dtos.Tickets;

public class UpdateStatusDto
{
    [Range(0, 2)] // 0=Open,1=InProgress,2=Resolved
    public int Status { get; set; }
}
