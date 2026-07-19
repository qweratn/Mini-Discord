using Backend.Domain.Common;

namespace Backend.Domain.Message;

public class Message : AggregateRoot
{
    private const int MaxContentLength = 2000;

    public Guid Id { get; private set; }

    public string Content { get; private set; } = null!;

    public Guid AuthorId { get; private set; }

    public Guid GuildId { get; private set; }

    public DateTimeOffset SendAt { get; private set; }

    public Message()
    {
    }

    public Message(string content, Guid authorId, Guid guildId)
    {
        Id = Guid.NewGuid();
        Content = content;
        AuthorId = authorId;
        GuildId = guildId;
        SendAt = DateTimeOffset.UtcNow;
    }

    public static Message Create(string content, Guid authorId, Guid guildId)
    {
        if (guildId == Guid.Empty)
        {
            throw new DomainException("Chat is required.");
        }

        if (authorId == Guid.Empty)
        {
            throw new DomainException("Author is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("Message cannot be empty.");
        }

        if (content.Length > MaxContentLength)
        {
            throw new DomainException($"Message cannot exceed {MaxContentLength} characters.");
        }

        Message msg = new Message(content, authorId, guildId);

        msg.AddDomainEvent(
            new MessageSentDomainEvent(msg.Content, msg.AuthorId, msg.GuildId, msg.SendAt));

        return msg;
    }
}
