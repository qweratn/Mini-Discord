using Backend.Domain.Users;

namespace Backend.Application.Users.RequestHandlers.Interfaces;

/// <summary>
/// Repository for User entities.
/// </summary>
public interface IUsersRepository
{
    Task<AppUser?> GetUserByClerkIdAsync(string clerkId);

    void AddUser(AppUser user);
}
