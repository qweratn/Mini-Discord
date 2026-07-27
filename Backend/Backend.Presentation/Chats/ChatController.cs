using Backend.Application.Chats.Models.Responses;
using Backend.Application.Chats.RequestHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Chats;

/// <summary>
/// Chat controller.
/// </summary>
[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator mediator;

    public ChatController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create a server chat.
    /// </summary>
    /// <response code="200">Successfully create.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("server")]
    public async Task<IActionResult> CreateServerChat(
        [FromBody] string chatName,
        CancellationToken cancellationToken)
    {
        string? clerkId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(clerkId))
        {
            return Unauthorized();
        }

        return Ok(await mediator.Send(
            new CreateServerChatCommand.Command(chatName, clerkId), cancellationToken));
    }

    /// <summary>
    /// Create a direct chat.
    /// </summary>
    /// <response code="200">Successfully create.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    [HttpPost("direct")]
    public async Task<IActionResult> CreateDirectChat(
        [FromBody] Guid companionId,
        CancellationToken cancellationToken)
    {
        string? clerkId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(clerkId))
        {
            return Unauthorized();
        }

        return Ok(await mediator.Send(
            new CreateDirectChatCommand.Command(clerkId, companionId), cancellationToken));
    }
}
