using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Controllers;

/// <summary>
/// Health controller.
/// </summary>
[ApiController]
[Route("api/users")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health endpoint for check
    /// that application works correctly.
    /// </summary>
    /// <response code="200" />
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }

    /// <summary>
    /// Health endpoint for check that
    /// application works correctly with authorized user.
    /// </summary>
    /// <response code="200" />
    /// <returns>User info.</returns>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        string? userId = User.FindFirst("sub")?.Value;

        if (userId == null)
        {
            throw new Exception("User not found");
        }

        return Ok(new
        {
            Id = userId,
            Username = User.FindFirst("username")?.Value,
            Email = User.FindFirst("email")?.Value,
        });
    }
}
