using Backend.Application.Users.Models;
using Backend.Application.Users.RequestHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Users;

/// <summary>
/// Users controller.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator mediator;

    public UserController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Insert users from clerk.
    /// </summary>
    /// <response code="200">Successfully insert.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    /// <response code="404">Username and email cannot be empty.</response>
    [HttpPut("sync")]
    public async Task<IActionResult> SyncUser(CancellationToken cancellationToken)
    {
        string? clerkId = User.FindFirst("sub")?.Value;
        string? username = User.FindFirst("username")?.Value;
        string? email = User.FindFirst("email")?.Value;
        string? imageUrl = User.FindFirst("imageUrl")?.Value;

        if (clerkId == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(imageUrl))
        {
            return BadRequest("Username, email, and image URL cannot be empty.");
        }

        UpsertAppUser appUser = new UpsertAppUser(clerkId, username, email, imageUrl);

        await mediator.Send(
            new SyncUserFromClerkCommand.Command(appUser), cancellationToken);
        return Ok();
    }
}
