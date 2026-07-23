using Backend.Domain.Common;

namespace Backend.Domain.Users;

/// <summary>
/// Represents a domain event that is raised when user registered in the system.
/// </summary>
public sealed record UserSynchronizedDomainEvent(
    Guid UserId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
