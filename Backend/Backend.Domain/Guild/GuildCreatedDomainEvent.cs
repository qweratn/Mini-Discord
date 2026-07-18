using Backend.Domain.Common;

namespace Backend.Domain.Guild;

public sealed record GuildCreatedDomainEvent(string Name, Guid OwnerId, DateTimeOffset OccurredAtUtc) : IDomainEvent;
