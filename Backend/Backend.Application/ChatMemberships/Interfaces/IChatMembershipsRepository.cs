using Backend.Domain.ChatMemberships;

namespace Backend.Application.ChatMemberships.Interfaces;

/// <summary>
/// Repository for ChatMembership entities.
/// </summary>
public interface IChatMembershipsRepository
{
    void AddChatMembership(ChatMembership chatMembership);
}
