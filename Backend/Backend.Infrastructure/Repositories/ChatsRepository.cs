using Backend.Application.Chats.Interfaces;
using Backend.Domain.Chats;
using Backend.Infrastructure.Data;

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
}
