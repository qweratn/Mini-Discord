using Backend.Application.Chats.Interfaces;
using Backend.Domain.Chats;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class ChatsRepository : IChatsRepository
{
    private readonly ApplicationDbContext context;

    public ChatsRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public void AddChat(Chat chat)
    {
        context.Chats.Add(chat);
    }

    public Task<Chat?> GetDirectChatByKey(Guid userId, Guid companionId, CancellationToken cancellationToken = default)
    {
        return context.Chats
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => c.DirectChatKey == Chat.GenerateDirectChatKey(userId, companionId),
                cancellationToken: cancellationToken);
    }
}
