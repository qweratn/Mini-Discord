using Backend.Application.ChatMemberships.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Infrastructure.Data;

namespace Backend.Infrastructure.Repositories;

public class ChatMembershipsRepository : IChatMembershipsRepository
{
    private readonly ApplicationDbContext context;

    public ChatMembershipsRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public void AddChatMembership(ChatMembership chatMembership)
    {
        context.ChatMemberships.Add(chatMembership);
    }
}
