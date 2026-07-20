using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Domain.Chats;

/// <summary>
/// Chat.
/// </summary>
public class Chat : AggregateRoot
{
    private const int MaxServerNameLength = 64;

    public Guid Id { get; private set; }

    public string? Name { get; private set; }

    public ChatType Type { get; private set; }

    // TODO: Add image url
    public Guid? OwnerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private Chat()
    {
    }

    private Chat(string name, ChatType type, Guid ownerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        OwnerId = ownerId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private Chat(ChatType type)
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
            throw new DomainException("Server name cannot be empty.");
        }

        if (name.Length > MaxServerNameLength)
        {
            throw new DomainException($"Server name cannot exceed {MaxServerNameLength} characters.");
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
