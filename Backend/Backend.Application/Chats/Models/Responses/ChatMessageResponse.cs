namespace Backend.Application.Chats.Models.Responses;
/// <summary>
/// Represents a response model for a chat message.
/// </summary>
/// <param name="Id">.</param>
/// <param name="ChatId">.</param>
/// <param name="Content">.</param>
/// <param name="Author">.</param>
/// <param name="SentAt">.</param>
public record ChatMessageResponse(
    Guid Id,
    Guid ChatId,
    string Content,
    MessageAuthorResponse Author,
    DateTimeOffset SentAt);
