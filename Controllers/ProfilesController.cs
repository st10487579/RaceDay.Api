
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public ProfilesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Profiles/me
    [HttpGet("me")]
    public async Task<ActionResult<object>> GetMyProfile()
    {
        var userId = SessionHelper.GetUserId(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to view your profile."
            });
        }

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserID == userId.Value)
            .Select(u => new
            {
                u.UserID,
                u.FullName,
                u.Email,
                u.Role
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // PUT: api/Profiles/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var userId = SessionHelper.GetUserId(HttpContext);

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Please log in to update your profile."
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserID == userId.Value);

        if (user == null)
        {
            return NotFound();
        }

        user.FullName = request.FullName;
        user.Email = request.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "That email address may already be in use."
            });
        }

        return Ok(new
        {
            message = "Profile updated successfully.",
            user.UserID,
            user.FullName,
            user.Email,
            user.Role
        });
    }
}

public class UpdateProfileRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    [System.ComponentModel.DataAnnotations.StringLength(100)]
    public string Email { get; set; } = string.Empty;
}
