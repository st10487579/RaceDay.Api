
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Category
{
    public int CategoryID { get; set; }

    public int EventID { get; set; }

    [Required, StringLength(50)]
    public string CategoryType { get; set; } = string.Empty;

    public int Age { get; set; }

    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
