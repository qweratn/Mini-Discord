using Backend.Application.ChatMemberships.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

/// <summary>
/// Repository for
/// <see cref="ChatMembership"/>.
/// </summary>
public class ChatMembershipsRepository : IChatMembershipsRepository
{
    private readonly ApplicationDbContext context;

    public ChatMembershipsRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Add membership to database.
    /// </summary>
    public void AddChatMembership(ChatMembership chatMembership)
    {
        context.ChatMemberships.Add(chatMembership);
    }

    /// <summary>
    /// Find memberships for the specified chats.
    /// </summary>
    public async Task<IReadOnlyList<ChatMembership>> GetByChatIdsAsync(
        IReadOnlyCollection<Guid> chatIds,
        CancellationToken cancellationToken)
    {
        if (chatIds.Count == 0)
        {
            return [];
        }

        return await context.ChatMemberships
            .Where(membership => chatIds.Contains(membership.ChatId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
