using Backend.Domain.Enums;

namespace Backend.Application.Chats.Models.Responses;

/// <summary>
/// Response model of user`s chat list item.
/// </summary>
/// <param name="ChatId">.</param>
/// <param name="Name">.</param>
/// <param name="ChatType">.</param>
/// <param name="ImageUrl">.</param>
/// <param name="LastMessage">.</param>
/// <param name="LastMessageAt">.</param>
public record UserChatListItem(
    Guid ChatId,
    string Name,
    ChatType ChatType,
    string? ImageUrl,
    string? LastMessage,
    DateTimeOffset? LastMessageAt);
