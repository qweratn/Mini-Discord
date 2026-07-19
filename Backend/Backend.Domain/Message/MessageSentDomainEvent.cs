using Backend.Domain.Common;

namespace Backend.Domain.Message;

/// <summary>
/// Represents a domain event that is raised when a message is sent in the system.
/// </summary>
public sealed record MessageSentDomainEvent(
    string Content,
    Guid AuthorId,
    Guid GuildId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
