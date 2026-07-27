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

    private ChatMembership()
    {
    }

    private ChatMembership(Guid chatId, Guid memberId)
    {
        ChatId = chatId;
        MemberId = memberId;
        JoinedAt = DateTimeOffset.UtcNow;
    }

    public static ChatMembership Create(Guid chatId, Guid memberId)
    {
        if (chatId == Guid.Empty)
        {
            throw new DomainException(
                "chat_membership.chat_required",
                "Chat ID cannot be empty.");
        }

        if (memberId == Guid.Empty)
        {
            throw new DomainException(
                "chat_membership.member_required",
                "Member ID cannot be empty.");
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
