
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public CategoriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        var role = SessionHelper.GetRole(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to view categories."
            });
        }

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/Categories/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var userId = SessionHelper.GetUserId(HttpContext);
        var role = SessionHelper.GetRole(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to view this category."
            });
        }

        if (role != "Organiser" && role != "Participant")
        {
            return Forbid();
        }

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] Category category)
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

        var eventRecord = await _context.Events
            .FirstOrDefaultAsync(e => e.EventID == category.EventID);

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

        category.CategoryID = 0;
        category.Event = null;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryID },
            category);
    }

    // PUT: api/Categories/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] Category updatedCategory)
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

        var category = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            return NotFound();
        }

        if (category.Event == null ||
            category.Event.UserID != userId.Value)
        {
            return Forbid();
        }

        if (updatedCategory.EventID != category.EventID)
        {
            return BadRequest(new
            {
                message = "A category cannot be moved to another event."
            });
        }

        category.CategoryType = updatedCategory.CategoryType;
        category.Age = updatedCategory.Age;

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    // DELETE: api/Categories/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
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

        var category = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
        {
            return NotFound();
        }

        if (category.Event == null ||
            category.Event.UserID != userId.Value)
        {
            return Forbid();
        }

        var hasEnrolments = await _context.Enrolments
            .AnyAsync(e => e.CategoryID == id);

        if (hasEnrolments)
        {
            return Conflict(new
            {
                message = "This category cannot be deleted because participants are enrolled in it."
            });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
