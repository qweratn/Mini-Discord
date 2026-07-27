using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Users;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext context;

    public UsersRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<AppUser?> GetUserByClerkIdAsync(string clerkId, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.ClerkId == clerkId, cancellationToken);
    }

    public async Task<AppUser?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public void AddUser(AppUser user)
    {
        context.Users.Add(user);
    }
}
