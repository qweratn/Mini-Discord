using Backend.Domain.Common;

namespace Backend.Domain.ChatMemberships;

/// <summary>
/// Relationship between a user and a chat, representing the user's membership in the chat.
/// </summary>
public class ChatMembership : AggregateRoot
{
    public Guid ChatId { get; private set; }

    public Guid MemberId { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public ChatMembership()
    {
    }

    public ChatMembership(Guid chatId, Guid memberId)
    {
        ChatId = chatId;
        MemberId = memberId;
        JoinedAt = DateTimeOffset.Now;
    }

    public static ChatMembership Create(Guid chatId, Guid memberId)
    {
        if (chatId == Guid.Empty)
        {
            throw new DomainException("Chat ID cannot be empty.");
        }

        if (memberId == Guid.Empty)
        {
            throw new DomainException("Member ID cannot be empty.");
        }

        ChatMembership chatMembership = new ChatMembership(chatId, memberId);

        chatMembership.AddDomainEvent(
            new ChatMemberJoinedDomainEvent(
                chatMembership.ChatId,
                chatMembership.MemberId,
                chatMembership.JoinedAt));

        return chatMembership;
    }
}
