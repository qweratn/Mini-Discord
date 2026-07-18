using Backend.Domain.Common;

namespace Backend.Domain.Guild;

/// <summary>
/// Guild (server).
/// </summary>
public class Guild : AggregateRoot
{
    private const int MaxGuildNameLength = 64;

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    // TODO: Add image url
    public Guid OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Guild()
    {
    }

    public Guild(string name, Guid ownerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        OwnerId = ownerId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Guild Create(string name, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Guild name cannot be empty.");
        }

        if (name.Length > MaxGuildNameLength)
        {
            throw new DomainException($"Guild name cannot exceed {MaxGuildNameLength} characters.");
        }

        if (ownerId == Guid.Empty)
        {
            throw new DomainException("Owner is required.");
        }

        Guild newGuild = new Guild(name, ownerId);

        newGuild.AddDomainEvent(
            new GuildCreatedDomainEvent(newGuild.Name, newGuild.OwnerId, newGuild.CreatedAt));

        return newGuild;
    }
}
