
namespace RaceDay.Api.Models;

public class Weather
{
    public int WeatherID { get; set; }

    public int EventID { get; set; }

    public decimal? Temperature { get; set; }

    public decimal? RainProbability { get; set; }

    public Event? Event { get; set; }
}
