namespace Backend.Application.Chats.Models.Responses;

/// <summary>
/// Represents info about a chat member.
/// </summary>
public record ChatMemberInfo(
    Guid Id,
    string Name,
    string Email,
    string ImageUrl);
