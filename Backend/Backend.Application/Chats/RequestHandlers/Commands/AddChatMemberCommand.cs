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
/// Add user to the chat.
/// </summary>
public class AddChatMemberCommand
{
    public record Command(string ActorUserId, Guid TargetUserId, Guid ChatId) : IRequest;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            (string actorUserId, Guid targetUserId, Guid chatId) = request;

            AppUser actorUser = await usersRepository.GetUserByClerkIdAsync(actorUserId, cancellationToken) ??
                                throw new NotFoundException(
                                    "user.not_found",
                                    "Actor user was not found.");

            AppUser targetUser = await usersRepository.GetUserByIdAsync(targetUserId, cancellationToken) ??
                                 throw new NotFoundException(
                                     "user.not_found",
                                     "Target user was not found.");

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

            if (chat.OwnerId != actorUser.Id)
            {
                throw new ConflictException(
                    "user.not_owner",
                    "Actor user is not a chat owner.");
            }

            IReadOnlyList<ChatMembership> chatMemberships = await membershipsRepository
                .GetByChatIdsAsync([chatId], cancellationToken);

            if (chatMemberships.Any(x => x.MemberId == targetUser.Id))
            {
                throw new ConflictException(
                    "membership.already_joined",
                    "Membership was already successfully joined.");
            }

            ChatMembership newMembership = ChatMembership.Create(chatId, targetUser.Id);
            membershipsRepository.AddChatMembership(newMembership);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
