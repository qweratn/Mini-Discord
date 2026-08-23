using System.Text.Json;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Common;
using Backend.Infrastructure.Outbox;

namespace Backend.UnitTests.Outbox;

public class OutboxEventSerializerTests
{
    private readonly Guid _chatId = Guid.NewGuid();
    private readonly Guid _memberId = Guid.NewGuid();
    private readonly DateTimeOffset _occuredAt = DateTimeOffset.UtcNow;
    private readonly OutboxEventSerializer _outboxEventSerializer = new();

    [Fact]
    public void HandleSerialize_ShouldSerialize()
    {
        ChatMemberJoinedDomainEvent domainEvent = new ChatMemberJoinedDomainEvent(
            _chatId,
            _memberId,
            _occuredAt);
        string expectedType = "chat-member-joined.v1";

        (string type, string content) = _outboxEventSerializer.Serialize(domainEvent);

        Assert.Equal(expectedType, type);
        Assert.Contains(_chatId.ToString(), content);
        Assert.Contains(_memberId.ToString(), content);
    }

    [Fact]
    public void HandleDeserialize_ShouldDeserialize()
    {
        ChatMemberJoinedDomainEvent expectedDomainEvent = new ChatMemberJoinedDomainEvent(
            _chatId,
            _memberId,
            _occuredAt);
        string type = "chat-member-joined.v1";
        string content = JsonSerializer.Serialize(expectedDomainEvent);

        IDomainEvent domainEvent = _outboxEventSerializer.Deserialize(type, content);

        Assert.Equal(expectedDomainEvent, domainEvent);
    }

    [Fact]
    public void HandleDeserialize_InvalidType_ShouldThrow()
    {
        string type = "invalid_type.v1";
        string content = "content";
        string exceptionMessage = $"No event type mapping found for name {type}";

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => _outboxEventSerializer.Deserialize(type, content));
        Assert.Equal(exceptionMessage, exception.Message);
    }
}
