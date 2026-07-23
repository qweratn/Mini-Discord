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

    public async Task<AppUser?> GetUserByClerkIdAsync(string clerkId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.ClerkId == clerkId);
    }

    public void AddUser(AppUser user)
    {
        context.Users.Add(user);
    }
}
