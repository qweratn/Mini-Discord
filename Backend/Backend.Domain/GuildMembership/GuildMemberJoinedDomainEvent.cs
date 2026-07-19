using Backend.Domain.Common;

namespace Backend.Domain.GuildMembership;

/// <summary>
/// Represents a domain event that is raised when user join to the guild.
/// </summary>
public sealed record GuildMemberJoinedDomainEvent(
    Guid GuildId,
    Guid MemberId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
