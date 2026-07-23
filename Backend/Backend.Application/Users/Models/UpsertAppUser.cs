namespace Backend.Application.Users.Models;

/// <summary>
/// Request model for upsert AppUser.
/// </summary>
public record UpsertAppUser(
    string ClerkId,
    string Username,
    string Email,
    string ImageUrl);
