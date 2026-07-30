using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Commands;

/// <summary>
/// Join the chat.
/// </summary>
public class JoinChatCommand
{
    public record Command(string ClerkId, Guid ChatId) : IRequest;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            (string clerkId, Guid chatId) = request;

            AppUser user = await usersRepository.GetUserByClerkIdAsync(clerkId, cancellationToken) ??
                           throw new NotFoundException(
                               "user.not_found",
                               "User was not found.");

            Chat chat = await chatsRepository.GetChatByIdAsync(chatId, cancellationToken) ??
                        throw new NotFoundException(
                            "chat.not_found",
                            "Chat was not found.");

            if (chat.Type is ChatType.Direct)
            {
                throw new ConflictException(
                    "chat.not_supported_type",
                    "Direct chat was not support.");
            }

            IReadOnlyList<ChatMembership> chatMembership = await membershipsRepository
                .GetByChatIdsAsync([chatId], cancellationToken);

            if (chatMembership.Any(x => x.MemberId == user.Id))
            {
                throw new ConflictException(
                    "membership.already_joined",
                    "Membership was already successfully joined.");
            }

            ChatMembership newChatMembership = ChatMembership.Create(chatId, user.Id);
            membershipsRepository.AddChatMembership(newChatMembership);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
