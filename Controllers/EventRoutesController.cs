
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventRoutesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EventRoutesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/EventRoutes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventRoute>>> GetEventRoutes()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        var role = SessionHelper.GetRole(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to view event routes."
            });
        }

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        var routes = await _context.EventRoutes
            .AsNoTracking()
            .ToListAsync();

        return Ok(routes);
    }

    // GET: api/EventRoutes/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventRoute>> GetEventRoute(int id)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        var role = SessionHelper.GetRole(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to view this route."
            });
        }

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        var route = await _context.EventRoutes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RouteID == id);

        if (route == null)
        {
            return NotFound();
        }

        return Ok(route);
    }

    // POST: api/EventRoutes
    [HttpPost]
    public async Task<IActionResult> CreateEventRoute(EventRoute newRoute)
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
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "Only organisers can create event routes." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var eventRecord = await _context.Events
            .FirstOrDefaultAsync(e => e.EventID == newRoute.EventID);

        if (eventRecord == null)
        {
            return BadRequest(new
            {
                message = "The selected event does not exist."
            });
        }

        if (eventRecord.UserID != userId.Value)
        {
            return Forbid();
        }

        if (newRoute.Distance <= 0)
        {
            return BadRequest(new
            {
                message = "Route distance must be greater than zero."
            });
        }

        newRoute.RouteID = 0;
        newRoute.Event = null;

        _context.EventRoutes.Add(newRoute);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEventRoute),
            new { id = newRoute.RouteID },
            newRoute);
    }
}
