using Backend.Domain.Enums;

namespace Backend.Application.Chats.Models.Responses;

/// <summary>
/// Response model of Chat.
/// </summary>
/// <param name="Id">.</param>
/// <param name="Name">.</param>
/// <param name="ChatType">.</param>
/// <param name="OwnerId">.</param>
/// <param name="CreatedAt">.</param>
public record ChatResponse(
    Guid Id,
    string Name,
    ChatType ChatType,
    Guid? OwnerId,
    DateTimeOffset CreatedAt);
