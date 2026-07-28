using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Users;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

/// <summary>
/// Repository for
/// <see cref="AppUser"/>.
/// </summary>
public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext context;

    public UsersRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Find user by Clerk Id.
    /// </summary>
    public async Task<AppUser?> GetUserByClerkIdAsync(
        string clerkId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.ClerkId == clerkId, cancellationToken);
    }

    /// <summary>
    /// Find user by User Id.
    /// </summary>
    public async Task<AppUser?> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Add user to Database.
    /// </summary>
    public void AddUser(AppUser user)
    {
        context.Users.Add(user);
    }
}
