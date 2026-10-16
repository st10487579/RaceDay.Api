
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public ResultsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Results
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Result>>> GetResults()
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

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        var query = _context.Results
            .AsNoTracking()
            .Include(r => r.Enrolment)
            .AsQueryable();

        // Participants can only view their own results.
        if (role == "Participant")
        {
            query = query.Where(r =>
                r.Enrolment != null &&
                r.Enrolment.UserID == userId.Value);
        }

        var results = await query.ToListAsync();

        return Ok(results);
    }

    // GET: api/Results/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result>> GetResult(int id)
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

        var result = await _context.Results
            .AsNoTracking()
            .Include(r => r.Enrolment)
            .FirstOrDefaultAsync(r => r.ResultID == id);

        if (result == null)
        {
            return NotFound();
        }

        if (role == "Participant" &&
            result.Enrolment?.UserID != userId.Value)
        {
            return Forbid();
        }

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        return Ok(result);
    }

    // POST: api/Results
    [HttpPost]
    public async Task<IActionResult> CreateResult(Result result)
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
                new
                {
                    message = "Only organisers can record results."
                });
        }

        var enrolment = await _context.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(
                e => e.EnrolmentID == result.EnrolmentID);

        if (enrolment == null)
        {
            return BadRequest(new
            {
                message = "The enrolment does not exist."
            });
        }

        // Check that the enrolment does not already have a result.
        var alreadyHasResult = await _context.Results
            .AnyAsync(r =>
                r.EnrolmentID == result.EnrolmentID);

        if (alreadyHasResult)
        {
            return Conflict(new
            {
                message = "A result already exists for this enrolment."
            });
        }

        // Organisers may only record results for their own events.
        if (enrolment.Event == null ||
            enrolment.Event.UserID != userId.Value)
        {
            return Forbid();
        }

        if (result.FinishTime <= TimeSpan.Zero)
        {
            return BadRequest(new
            {
                message = "Finish time must be greater than zero."
            });
        }

        // Set server-controlled values.
        result.ResultID = 0;
        result.RecordedAt = DateTime.UtcNow;
        result.Enrolment = null;

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetResult),
            new { id = result.ResultID },
            result);
    }
}
