using Backend.Domain.Common;

namespace Backend.Domain.User;

public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
