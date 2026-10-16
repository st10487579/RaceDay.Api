
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Event
{
    public int EventID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required, StringLength(100)]
    public string EventName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required, StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string EventType { get; set; } = string.Empty;

    public User? Organiser { get; set; }

    public ICollection<Weather> WeatherRecords { get; set; } = new List<Weather>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<EventRoute> Routes { get; set; } = new List<EventRoute>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
