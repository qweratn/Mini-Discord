using Backend.Domain.Common;

namespace Backend.Domain.Message;

public sealed record MessageSentDomainEvent(
    string Content,
    Guid AuthorId,
    Guid GuildId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
