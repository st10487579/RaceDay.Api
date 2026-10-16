
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EventsController(RaceDayContext context)
    { 

        _context = context;

    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent(Event newEvent)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        var role = SessionHelper.GetRole(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in first."
            });
        }

        if (role != "Organiser")
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        newEvent.UserID = userId.Value;

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvent),
            new { id = newEvent.EventID },
            newEvent);
    }


    // GET: api/Events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/Events/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        var raceEvent = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventID == id);

        if (raceEvent == null)
        {
            return NotFound();
        }

        return Ok(raceEvent);

        if (SessionHelper.GetUserId(HttpContext) == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to access events."
            });
        }
    }
}
