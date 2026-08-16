using Backend.Application.SignalR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Presentation.Hubs;

/// <summary>
/// SignalR hub for chat functionality.
/// </summary>
[Authorize]
public class ChatHub : Hub<IChatClient>
{
}
