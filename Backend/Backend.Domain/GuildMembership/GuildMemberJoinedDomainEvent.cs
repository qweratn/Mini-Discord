using Backend.Domain.Common;

namespace Backend.Domain.GuildMembership;

public sealed record GuildMemberJoinedDomainEvent(
    Guid GuildId,
    Guid MemberId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
