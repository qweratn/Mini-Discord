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
    /// Method of clearing domain events from the aggregate root.
    /// </summary>
    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    /// <summary>
    /// Method of adding a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">Domain event.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
