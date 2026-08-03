using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Request;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Messages.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Commands;

/// <summary>
/// Sends a message to the chat.
/// </summary>
public class SendMessageCommand
{
    public record Command(SendMessage SendMessage) : IRequest;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IMessagesRepository messagesRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            SendMessage sendMessage = request.SendMessage;

            AppUser author = (await usersRepository.GetUserByClerkIdAsync(sendMessage.AuthorClerkId, cancellationToken)) ??
                             throw new NotFoundException(
                                 "user.not_found",
                                 "Author user was not found.");

            Chat chat = await chatsRepository.GetChatByIdAsync(sendMessage.ChatId, cancellationToken) ??
                        throw new NotFoundException(
                            "chat.not_found",
                            "Chat was not found.");

            IReadOnlyList<ChatMembership> membership = await membershipsRepository.GetByChatIdsAsync([chat.Id], cancellationToken) ??
                        throw new NotFoundException(
                            "membership.not_found",
                            "Author is not a member of this chat.");

            if (membership.All(x => x.MemberId != author.Id))
            {
                throw new ConflictException(
                    "user.not_member",
                    "Author is not a member of this chat.");
            }

            Message message = Message.Create(
                sendMessage.Content,
                author.Id,
                chat.Id);

            messagesRepository.AddMessage(message);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
