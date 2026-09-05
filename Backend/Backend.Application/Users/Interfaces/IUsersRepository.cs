using Backend.Domain.Users;

namespace Backend.Application.Users.Interfaces;

/// <summary>
/// Repository for User entities.
/// </summary>
public interface IUsersRepository
{
    Task<AppUser?> GetUserByClerkIdAsync(string clerkId, CancellationToken cancellationToken);

    Task<AppUser?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Dictionary<Guid, AppUser>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppUser>> GetUsersByQueryAsync(
        string clerkId,
        string query,
        CancellationToken cancellationToken);

    void AddUser(AppUser user);
}
