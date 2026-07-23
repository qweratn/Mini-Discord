using Backend.Domain.Common;

namespace Backend.Domain.Users;

/// <summary>
/// User.
/// </summary>
public class AppUser : AggregateRoot
{
    private const int MaxUsernameLength = 32;

    public Guid Id { get; private set; }

    public string ClerkId { get; private set; } = null!;

    public string Username { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    private AppUser()
    {
    }

    private AppUser(
        Guid id,
        string clerkId,
        string username,
        string email,
        DateTimeOffset createdAt)
    {
        Id = id;
        ClerkId = clerkId;
        Username = username;
        Email = email;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Create new user.
    /// </summary>
    public static AppUser SyncFromClerk(
        string clerkId,
        string username,
        string email)
    {
        if (string.IsNullOrEmpty(clerkId))
        {
            throw new DomainException("Click ID cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException("Username cannot be empty.");
        }

        if (username.Length > MaxUsernameLength)
        {
            throw new DomainException($"Username cannot exceed {MaxUsernameLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty.");
        }

        AppUser newAppUser = new AppUser(
            Guid.NewGuid(),
            clerkId,
            username,
            email,
            DateTimeOffset.UtcNow);

        newAppUser.AddDomainEvent(new UserSynchronizedDomainEvent(newAppUser.Id, newAppUser.CreatedAt));

        return newAppUser;
    }

    /// <summary>
    /// Update user info.
    /// </summary>
    public void SyncProfile(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException("Username cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty.");
        }

        Username = username.Trim();
        Email = email.Trim();
    }
}
