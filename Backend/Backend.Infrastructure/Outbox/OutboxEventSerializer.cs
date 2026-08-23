using System.Text.Json;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Common;
using Backend.Domain.Messages;
using Backend.Domain.Users;

namespace Backend.Infrastructure.Outbox;

/// <summary>
/// Serializer for <see cref="OutboxMessage"/>.
/// </summary>
public class OutboxEventSerializer
{
    private static readonly JsonSerializerOptions Options =
        new(JsonSerializerDefaults.Web);

    private static readonly IReadOnlyDictionary<Type, string> EventNames =
        new Dictionary<Type, string>
        {
            [typeof(ChatMemberJoinedDomainEvent)] =
                "chat-member-joined.v1",

            [typeof(ChatCreatedDomainEvent)] =
                "chat-created.v1",

            [typeof(MessageSentDomainEvent)] =
                "message-sent.v1",

            [typeof(UserSynchronizedDomainEvent)] =
                "user-synchronized.v1",
        };

    private static IReadOnlyDictionary<string, Type> EventTypes =>
        EventNames.ToDictionary(kv => kv.Value, kv => kv.Key);

    public (string Type, string Content) Serialize(IDomainEvent domainEvent)
    {
        Type runtimeType = domainEvent.GetType();

        if (!EventNames.TryGetValue(runtimeType, out string? eventName))
        {
            throw new InvalidOperationException(
                $"No event name mapping found for type {runtimeType.Name}");
        }

        string content = JsonSerializer.Serialize(
            domainEvent,
            runtimeType,
            Options);

        return (eventName, content);
    }

    public IDomainEvent Deserialize(string type, string content)
    {
        if (!EventTypes.TryGetValue(type, out Type? eventType))
        {
            throw new InvalidOperationException(
                $"No event type mapping found for name {type}");
        }

        return JsonSerializer.Deserialize(content, eventType, Options) as IDomainEvent ??
               throw new InvalidOperationException(
                   $"Could not deserialize '{type}'.");
    }
}
