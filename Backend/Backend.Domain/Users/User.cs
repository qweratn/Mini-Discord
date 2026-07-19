using Backend.Domain.Common;

namespace Backend.Domain.Users;

/// <summary>
/// User.
/// </summary>
public class User : AggregateRoot
{
    private const int MaxUsernameLength = 32;

    public Guid Id { get; private set; }

    public string Username { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        string username,
        string email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Register(
        string username,
        string email,
        string passwordHash)
    {
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

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash cannot be empty.");
        }

        User newUser = new User(
            Guid.NewGuid(),
            username,
            email,
            passwordHash,
            DateTimeOffset.UtcNow);

        newUser.AddDomainEvent(new UserRegisteredDomainEvent(newUser.Id, newUser.CreatedAt));

        return newUser;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new DomainException("Password hash cannot be empty.");
        }

        PasswordHash = newPasswordHash;
    }
}
