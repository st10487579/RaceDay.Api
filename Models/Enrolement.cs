
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Enrolment
{
    public int EnrolmentID { get; set; }

    public int UserID { get; set; }

    public int EventID { get; set; }

    public int CategoryID { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Active";

    public User? User { get; set; }
    public Event? Event { get; set; }
    public Category? Category { get; set; }

    public Result? Result { get; set; }
}
