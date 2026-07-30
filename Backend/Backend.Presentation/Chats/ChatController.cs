using Backend.Application.Chats.RequestHandlers.Commands;
using Backend.Application.Chats.RequestHandlers.Queries;
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

    /// <summary>
    /// Get user`s chats.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    /// <response code="404">
    /// Possible reasons:
    /// - User was not found.
    /// - Companion was not found.
    /// </response>
    /// <response code="409">
    /// Possible reasons:
    /// - Direct chat must contain the current user and exactly one companion.
    /// - Server chat name is missing.
    /// - Direct chat companion is missing.
    /// </response>
    [HttpGet]
    public async Task<IActionResult> GetUserChats(CancellationToken cancellationToken)
    {
        string? clerkId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(clerkId))
        {
            return Unauthorized();
        }

        return Ok(await mediator.Send(
            new GetUserChatsQuery.Query(clerkId), cancellationToken));
    }

    /// <summary>
    /// Get user`s chats.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="401">The request has no valid Clerk token.</response>
    /// <response code="404">Chat was not found.</response>
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatInfo(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        string? clerkId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(clerkId))
        {
            return Unauthorized();
        }

        return Ok(await mediator.Send(
            new GetChatInfoQuery.Query(clerkId, chatId), cancellationToken));
    }
}
