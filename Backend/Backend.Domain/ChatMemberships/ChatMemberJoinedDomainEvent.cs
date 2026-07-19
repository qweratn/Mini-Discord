using Backend.Domain.Common;

namespace Backend.Domain.ChatMemberships;

/// <summary>
/// Represents a domain event that is raised when a user joins a chat.
/// </summary>
public sealed record ChatMemberJoinedDomainEvent(
    Guid ChatId,
    Guid MemberId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
