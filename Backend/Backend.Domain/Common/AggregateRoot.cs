namespace Backend.Domain.Common;

/// <summary>
/// Base class for aggregate roots that record domain events.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    /// <summary>
    /// Domain events.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
    
    /// <summary>
    /// Method of adding a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">Domain event</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// Method of clearing domain events from the aggregate root.
    /// </summary>
    protected void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
