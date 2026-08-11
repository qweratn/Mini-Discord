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

    public async Task<Message?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return await context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> GetChatMessagesAsync(
        Guid chatId,
        DateTimeOffset? beforeSentAt,
        Guid? beforeMessageId,
        int take,
        CancellationToken cancellationToken)
    {
        IQueryable<Message> query = context.Messages
            .Where(x => x.ChatId == chatId);

        if (beforeSentAt.HasValue &&
            beforeMessageId.HasValue)
        {
            query = query.Where(message =>
                message.SendAt < beforeSentAt.Value ||
                (message.SendAt == beforeSentAt.Value &&
                 message.Id.CompareTo(beforeMessageId.Value) < 0));
        }

        return await query
            .OrderByDescending(x => x.SendAt)
            .ThenByDescending(x => x.Id)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
