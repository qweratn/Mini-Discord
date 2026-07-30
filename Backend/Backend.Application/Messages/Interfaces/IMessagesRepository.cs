using Backend.Domain.Messages;

namespace Backend.Application.Messages.Interfaces;

public interface IMessagesRepository
{
    Task<IReadOnlyDictionary<Guid, Message>> GetLastMessagesAsync(
        IReadOnlyCollection<Guid> chatIds,
        CancellationToken cancellationToken);

    void AddMessage(Message message);
}
