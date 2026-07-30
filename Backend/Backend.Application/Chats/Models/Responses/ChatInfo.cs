using Backend.Domain.Enums;

namespace Backend.Application.Chats.Models.Responses;

/// <summary>
/// Represents chat info.
/// </summary>
public record ChatInfo(
    Guid Id,
    string Name,
    ChatType ChatType,
    string? ImageUrl,
    int MembersCount);
