using Backend.Domain.ChatMemberships;

namespace Backend.Application.ChatMemberships.Interfaces;

/// <summary>
/// Repository for ChatMembership entities.
/// </summary>
public interface IChatMembershipsRepository
{
    void AddChatMembership(ChatMembership chatMembership);

    Task<IReadOnlyList<ChatMembership>> GetByChatIdsAsync(
        IReadOnlyCollection<Guid> chatIds,
        CancellationToken cancellationToken);
}
