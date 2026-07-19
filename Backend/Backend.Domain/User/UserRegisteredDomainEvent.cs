using Backend.Domain.Common;

namespace Backend.Domain.User;

/// <summary>
/// Represents a domain event that is raised when user registered in the system.
/// </summary>
public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
