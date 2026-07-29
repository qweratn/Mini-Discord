using Backend.Application.Chats.Interfaces;
using Backend.Domain.Chats;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

/// <summary>
/// Repository for
/// <see cref="Chat"/>.
/// </summary>
public class ChatsRepository : IChatsRepository
{
    private readonly ApplicationDbContext context;

    public ChatsRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Add chat to database.
    /// </summary>
    public void AddChat(Chat chat)
    {
        context.Chats.Add(chat);
    }

    /// <summary>
    /// Find chat by ChatKey.
    /// </summary>
    public Task<Chat?> GetDirectChatByKey(
        Guid userId,
        Guid companionId,
        CancellationToken cancellationToken = default)
    {
        return context.Chats
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => c.DirectChatKey == Chat.GenerateDirectChatKey(userId, companionId),
                cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Chat>> GetUserChatsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await context.ChatMemberships
            .Where(membership => membership.MemberId == userId)
            .Join(
                context.Chats,
                membership => membership.ChatId,
                chat => chat.Id,
                (_, chat) => chat)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
