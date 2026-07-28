using Backend.Domain.Chats;

namespace Backend.Application.Chats.Interfaces;

/// <summary>
/// Repository for Chat entities.
/// </summary>
public interface IChatsRepository
{
    void AddChat(Chat chat);

    Task<Chat?> GetDirectChatByKey(Guid userId, Guid companionId, CancellationToken cancellationToken);
}
