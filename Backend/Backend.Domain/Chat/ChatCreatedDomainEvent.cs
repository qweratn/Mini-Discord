using Backend.Domain.Common;

namespace Backend.Domain.Guild;

/// <summary>
/// Represents a domain event that is raised when a guild created in the system.
/// </summary>
public sealed record ChatCreatedDomainEvent(
    string Name,
    Guid OwnerId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
