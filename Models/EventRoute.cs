
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class EventRoute
{
    [Key]
    public int RouteID { get; set; }

    public int EventID { get; set; }

    [Required]
    [StringLength(100)]
    public string RouteName { get; set; } = string.Empty;

    public decimal Distance { get; set; }

    public Event? Event { get; set; }
}
