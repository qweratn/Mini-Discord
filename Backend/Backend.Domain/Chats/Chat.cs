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

    public string? DirectChatKey { get; private set; }

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

    private Chat(ChatType type, Guid userId1, Guid userId2)
    {
        Id = Guid.NewGuid();
        Name = null;
        Type = type;
        OwnerId = null;
        DirectChatKey = GenerateDirectChatKey(userId1, userId2);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Chat CreateServer(string name, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "chat.server_name_empty",
                "Server name cannot be empty.");
        }

        if (name.Length > MaxServerNameLength)
        {
            throw new DomainException(
                "chat.server_name_too_long",
                $"Server name cannot exceed {MaxServerNameLength} characters.");
        }

        if (ownerId == Guid.Empty)
        {
            throw new DomainException(
                "chat.owner_required",
                "Owner is required.");
        }

        Chat newServerChat = new Chat(name, ChatType.Server, ownerId);

        newServerChat.AddDomainEvent(
            new ChatCreatedDomainEvent(name, ownerId, newServerChat.CreatedAt));

        return newServerChat;
    }

    public static Chat CreateDirect(Guid userId1, Guid userId2)
    {
        if (userId1 == Guid.Empty || userId2 == Guid.Empty)
        {
            throw new DomainException(
                "chat.direct_users_required",
                "Users are required.");
        }

        if (userId1 == userId2)
        {
            throw new DomainException(
                "chat.direct_with_self",
                "A direct chat cannot be created with the same user.");
        }

        return new Chat(ChatType.Direct, userId1, userId2);
    }

    public static string GenerateDirectChatKey(Guid userId1, Guid userId2)
    {
        List<Guid> orderedIds = new List<Guid> { userId1, userId2 }.OrderBy(id => id).ToList();
        return $"{orderedIds[0]}_{orderedIds[1]}";
    }
}
