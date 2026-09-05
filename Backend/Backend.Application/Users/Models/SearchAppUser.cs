namespace Backend.Application.Users.Models;

/// <summary>
/// Request model for search AppUser.
/// </summary>
public record SearchAppUser(
    Guid Id,
    string Username,
    string Email,
    string ImageUrl);
