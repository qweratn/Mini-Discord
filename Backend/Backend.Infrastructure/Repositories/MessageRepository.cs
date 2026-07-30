using Backend.Application.Messages.Interfaces;
using Backend.Domain.Messages;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class MessageRepository : IMessagesRepository
{
    private readonly ApplicationDbContext context;

    public MessageRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyDictionary<Guid, Message>> GetLastMessagesAsync(
        IReadOnlyCollection<Guid> chatIds,
        CancellationToken cancellationToken)
    {
        if (chatIds.Count == 0)
        {
            return new Dictionary<Guid, Message>();
        }

        List<Message> messages = await context.Messages
            .Where(x => chatIds.Contains(x.ChatId))
            .GroupBy(x => x.ChatId)
            .Select(g => g
                .OrderByDescending(x => x.SendAt)
                .ThenByDescending(x => x.Id)
                .First())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return messages.ToDictionary(
            x => x.ChatId,
            x => x);
    }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }
}
