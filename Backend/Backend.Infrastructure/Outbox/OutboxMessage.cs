namespace Backend.Infrastructure.Outbox;

/// <summary>
/// Represents a message in the outbox for reliable message delivery.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; private set; }

    public string Type { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public DateTimeOffset NextAttemptAtUtc { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public DateTimeOffset? FailedAtUtc { get; private set; }

    public int RetryCount { get; private set; }

    public string? Error { get; private set; }

    private OutboxMessage()
    {
    }

    public static OutboxMessage Create(
        string type,
        string content,
        DateTimeOffset occurredAtUtc)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Content = content,
            OccurredAtUtc = occurredAtUtc,
            NextAttemptAtUtc = DateTimeOffset.UtcNow,
        };
    }

    public void MarkProcessed(DateTimeOffset processedAtUtc)
    {
        ProcessedAtUtc = processedAtUtc;
        Error = null;
    }

    public void MarkFailed(Exception exception, DateTimeOffset nextAttemptAtUtc)
    {
        RetryCount++;
        Error = exception.ToString();
        NextAttemptAtUtc = nextAttemptAtUtc;
    }

    public void MoveToDeadLetter(DateTimeOffset failedAtUtc)
    {
        FailedAtUtc = failedAtUtc;
    }
}
