namespace Backend.Domain.Common;

/// <summary>
/// Interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Time when the domain event occurred.
    /// </summary>
    DateTimeOffset OccurredAtUtc { get; }
}
