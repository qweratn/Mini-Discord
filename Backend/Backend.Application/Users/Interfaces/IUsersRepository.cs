using Backend.Domain.Users;

namespace Backend.Application.Users.Interfaces;

/// <summary>
/// Repository for User entities.
/// </summary>
public interface IUsersRepository
{
    Task<AppUser?> GetUserByClerkIdAsync(string clerkId, CancellationToken cancellationToken);

    Task<AppUser?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);

    void AddUser(AppUser user);
}
