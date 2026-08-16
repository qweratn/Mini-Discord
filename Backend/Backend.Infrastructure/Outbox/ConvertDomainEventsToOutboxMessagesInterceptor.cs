using Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.Infrastructure.Outbox;

/// <summary>
/// An interceptor that converts domain events to outbox messages before saving changes to the database.
/// </summary>
public class ConvertDomainEventsToOutboxMessagesInterceptor
    (OutboxEventSerializer serializer)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        MoveDomainEventsToOutbox(eventData.Context);

        return result;
    }

    public override ValueTask<InterceptionResult<int>>
        SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
    {
        MoveDomainEventsToOutbox(eventData.Context);

        return ValueTask.FromResult(result);
    }

    private void MoveDomainEventsToOutbox(
        DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        List<AggregateRoot> aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate =>
                aggregate.DomainEvents.Count > 0)
            .ToList();

        if (aggregates.Count == 0)
        {
            return;
        }

        List<IDomainEvent> domainEvents = aggregates
            .SelectMany(aggregate =>
                aggregate.DomainEvents)
            .ToList();

        List<OutboxMessage> outboxMessages = domainEvents
            .Select(CreateOutboxMessage)
            .ToList();

        context.Set<OutboxMessage>()
            .AddRange(outboxMessages);

        foreach (AggregateRoot aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }
    }

    private OutboxMessage CreateOutboxMessage(
        IDomainEvent domainEvent)
    {
        (string type, string content) =
            serializer.Serialize(domainEvent);

        return OutboxMessage.Create(
            type,
            content,
            domainEvent.OccurredAtUtc);
    }
}
