using Backend.Domain.Common;

namespace Backend.Domain.Messages;

/// <summary>
/// Represents a domain event that is raised when a message is sent in the system.
/// </summary>
public sealed record MessageSentDomainEvent(
    Guid MessageId,
    string Content,
    Guid AuthorId,
    Guid ChatId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
