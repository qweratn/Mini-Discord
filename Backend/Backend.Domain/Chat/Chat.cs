using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Domain.Guild;

/// <summary>
/// Guild (server).
/// </summary>
public class Chat : AggregateRoot
{
    private const int MaxGuildNameLength = 64;

    public Guid Id { get; private set; }

    public string? Name { get; private set; }

    public ChatType Type { get; private set; }

    // TODO: Add image url
    public Guid? OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Chat()
    {
    }

    public Chat(string name, ChatType type, Guid ownerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        OwnerId = ownerId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Chat(ChatType type)
    {
        Id = Guid.NewGuid();
        Name = null;
        Type = type;
        OwnerId = null;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Chat CreateServer(string name, Guid ownerId)
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

        Chat newServerChat = new Chat(name, ChatType.Server, ownerId);

        newServerChat.AddDomainEvent(
            new ChatCreatedDomainEvent(name, ownerId, newServerChat.CreatedAt));

        return newServerChat;
    }

    public static Chat CreateDirect() => new Chat(ChatType.Direct);
}
