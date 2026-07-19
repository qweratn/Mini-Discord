using Backend.Domain.Common;

namespace Backend.Domain.Chats;

/// <summary>
/// Represents a domain event that is raised when a chat is created in the system.
/// </summary>
public sealed record ChatCreatedDomainEvent(
    string Name,
    Guid OwnerId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;
