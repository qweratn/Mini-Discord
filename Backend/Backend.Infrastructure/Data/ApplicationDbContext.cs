using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data;

/// <summary>
/// Application database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Chat> Chats { get; set; }

    public DbSet<ChatMembership> ChatMemberships { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
