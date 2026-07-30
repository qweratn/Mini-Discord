using Backend.Domain.Chats;

namespace Backend.Application.Chats.Interfaces;

/// <summary>
/// Repository for Chat entities.
/// </summary>
public interface IChatsRepository
{
    void AddChat(Chat chat);

    Task<Chat?> GetDirectChatByKey(Guid userId, Guid companionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Chat>> GetUserChatsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Chat?> GetChatInfoAsync(Guid chatId, CancellationToken cancellationToken);

    Task<int> GetChatMembersCount(Guid chatId, CancellationToken cancellationToken);
}
