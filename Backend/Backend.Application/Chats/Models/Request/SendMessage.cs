namespace Backend.Application.Chats.Models.Request;

/// <summary>
/// Represents a request model for sending a message.
/// </summary>
public record SendMessage(
    string Content,
    string AuthorClerkId,
    Guid ChatId);
