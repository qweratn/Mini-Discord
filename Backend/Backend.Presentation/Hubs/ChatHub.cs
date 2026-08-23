using Backend.Application.Chats.RequestHandlers.Queries;
using Backend.Application.SignalR.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Presentation.Hubs;

/// <summary>
/// SignalR hub for chat functionality.
/// </summary>
[Authorize]
public class ChatHub(IMediator mediator) : Hub<IChatClient>
{
    public async Task SubscribeToChat(Guid chatId)
    {
        string? clerkId =
            Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(clerkId))
        {
            throw new HubException(
                "User is not authenticated.");
        }

        await mediator.Send(
            new GetChatInfoQuery.Query(
                clerkId,
                chatId),
            Context.ConnectionAborted);

        string groupName = $"chat:{chatId:N}";

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            groupName,
            Context.ConnectionAborted);
    }
}
