namespace Backend.Application.Chats.Models.Responses;
/// <summary>
/// Represents a response model for the author of a message.
/// </summary>
/// <param name="Id">.</param>
/// <param name="Username">.</param>
/// <param name="ImageUrl">.</param>
public record MessageAuthorResponse(
    Guid Id,
    string Username,
    string ImageUrl);
