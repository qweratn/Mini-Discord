namespace Backend.Application.Chats.Models.Responses;

/// <summary>
/// Represents a response model for retrieving chat messages, including pagination information.
/// </summary>
/// <param name="Items">.</param>
/// <param name="NextBeforeMessageId">.</param>
/// <param name="HasMore">.</param>
public record GetChatMessageResponse(
    IReadOnlyList<ChatMessageResponse> Items,
    Guid? NextBeforeMessageId,
    bool HasMore);
