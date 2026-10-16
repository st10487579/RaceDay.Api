
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Helpers;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EnrolmentsController(RaceDayContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrolment(Enrolment enrolment)
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

        if (role != "Participant")
        {
            return StatusCode(403, new
            {
                message = "Only participants can enrol in events."
            });
        }

        var eventExists = await _context.Events
            .AnyAsync(e => e.EventID == enrolment.EventID);

        if (!eventExists)
        {
            return BadRequest(new
            {
                message = "The selected event does not exist."
            });
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c =>
                c.CategoryID == enrolment.CategoryID &&
                c.EventID == enrolment.EventID);

        if (!categoryExists)
        {
            return BadRequest(new
            {
                message = "The selected category does not belong to this event."
            });
        }

        var alreadyEnrolled = await _context.Enrolments
            .AnyAsync(e =>
                e.UserID == userId.Value &&
                e.EventID == enrolment.EventID);

        if (alreadyEnrolled)
        {
            return Conflict(new
            {
                message = "You are already enrolled in this event."
            });
        }

        enrolment.UserID = userId.Value;
        enrolment.EnrolmentDate = DateTime.UtcNow;
        enrolment.Status = "Active";

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEnrolment),
            new { id = enrolment.EnrolmentID },
            enrolment);
    }


    // GET: api/Enrolments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enrolment>>> GetEnrolments()
    {
        var enrolments = await _context.Enrolments
            .AsNoTracking()
            .ToListAsync();

        return Ok(enrolments);
    }

    // GET: api/Enrolments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Enrolment>> GetEnrolment(int id)
    {
        var enrolment = await _context.Enrolments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrolmentID == id);

        if (enrolment == null)
        {
            return NotFound();
        }

        return Ok(enrolment);

    }
}
