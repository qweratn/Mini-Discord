using Backend.Application.Chats.Models.Responses;

namespace Backend.Application.SignalR.Interfaces;

/// <summary>
/// Describes events that the server can send to a connected chat client.
/// </summary>
public interface IChatClient
{
    /// <summary>
    /// Notifies the client that a new message has been created.
    /// </summary>
    Task MessageReceived(ChatMessageResponse message);

    /// <summary>
    /// Notifies the client that a chat member has joined the chat.
    /// </summary>
    Task ChatMemberJoined(Guid chatId, ChatMemberInfo member);
}
