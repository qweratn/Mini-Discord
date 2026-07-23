using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Health;

/// <summary>
/// Health controller.
/// </summary>
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health endpoint for check
    /// that application works correctly.
    /// </summary>
    /// <response code="200">Returns OK if application works correctly.</response>
    [HttpGet]
    public IActionResult Health()
    {
        return Ok();
    }

    /// <summary>
    /// Health endpoint for check that
    /// application works correctly with authorized user.
    /// </summary>
    /// <response code="200">Returns the current user.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        string? clerkId = User.FindFirst("sub")?.Value;

        if (clerkId == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            Id = clerkId,
            Username = User.FindFirst("username")?.Value,
            Email = User.FindFirst("email")?.Value,
        });
    }
}
