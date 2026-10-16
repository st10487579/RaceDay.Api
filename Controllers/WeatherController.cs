
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public WeatherController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Weather
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weather>>> GetWeather()
        {
            var userId = SessionHelper.GetUserId(HttpContext);
            var role = SessionHelper.GetRole(HttpContext);

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in to view weather records."
                });
            }

            if (role != "Organiser" && role != "Participant")
            {
                return Forbid();
            }

            var weatherRecords = await _context.Weather
                .AsNoTracking()
                .ToListAsync();

            return Ok(weatherRecords);
        }

        // GET: api/Weather/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Weather>> GetWeatherById(int id)
        {
            var userId = SessionHelper.GetUserId(HttpContext);
            var role = SessionHelper.GetRole(HttpContext);

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in to view this weather record."
                });
            }

            if (role != "Organiser" && role != "Participant")
            {
                return Forbid();
            }

            var weather = await _context.Weather
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WeatherID == id);

            if (weather == null)
            {
                return NotFound();
            }

            return Ok(weather);
        }
    }



