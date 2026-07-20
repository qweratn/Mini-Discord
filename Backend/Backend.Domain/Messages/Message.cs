using Backend.Domain.Common;

namespace Backend.Domain.Messages;

public class Message : AggregateRoot
{
    private const int MaxContentLength = 2000;

    public Guid Id { get; private set; }

    public string Content { get; private set; } = null!;

    public Guid AuthorId { get; private set; }

    public Guid ChatId { get; private set; }

    public DateTimeOffset SendAt { get; private set; }

    private Message()
    {
    }

    private Message(string content, Guid authorId, Guid chatId)
    {
        Id = Guid.NewGuid();
        Content = content;
        AuthorId = authorId;
        ChatId = chatId;
        SendAt = DateTimeOffset.UtcNow;
    }

    public static Message Create(string content, Guid authorId, Guid chatId)
    {
        if (chatId == Guid.Empty)
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

        Message msg = new Message(content, authorId, chatId);

        msg.AddDomainEvent(
            new MessageSentDomainEvent(msg.Content, msg.AuthorId, msg.ChatId, msg.SendAt));

        return msg;
    }
}
