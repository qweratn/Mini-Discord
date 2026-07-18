namespace Backend.Domain.Common;

/// <summary>
/// Base class for aggregate roots that record domain events.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> domainEvents = [];

    /// <summary>
    /// Domain events.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => this.domainEvents;

    /// <summary>
    /// Method of adding a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">Domain event.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        this.domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Method of clearing domain events from the aggregate root.
    /// </summary>
    protected void ClearDomainEvents()
    {
        this.domainEvents.Clear();
    }
}
