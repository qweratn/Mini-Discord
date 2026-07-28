using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Common.Exceptions;
using Backend.Application.Common.Interfaces;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Commands;

/// <summary>
/// Command to create a new direct chat.
/// </summary>
public class CreateDirectChatCommand
{
    public record Command(string ClerkId, Guid CompanionUserId) : IRequest<ChatResponse>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<Command, ChatResponse>
    {
        public async Task<ChatResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            (string clerkId, Guid companionUserId) = request;

            AppUser owner = await usersRepository
                                .GetUserByClerkIdAsync(clerkId, cancellationToken)
                            ?? throw new NotFoundException(
                                "user.not_found",
                                "Owner was not found.");

            AppUser companion = await usersRepository.GetUserByIdAsync(companionUserId, cancellationToken)
                               ?? throw new NotFoundException(
                                   "user.not_found",
                                   "Companion user was not found.");

            Chat? existingChat = await chatsRepository
                .GetDirectChatByKey(owner.Id, companion.Id, cancellationToken);

            if (existingChat != null)
            {
                return new ChatResponse(
                    existingChat.Id,
                    existingChat.Name,
                    existingChat.Type,
                    existingChat.OwnerId,
                    existingChat.CreatedAt);
            }

            Chat directChat = Chat.CreateDirect(owner.Id, companion.Id);
            chatsRepository.AddChat(directChat);

            ChatMembership ownerMembership = ChatMembership.Create(directChat.Id, owner.Id);
            ChatMembership companionMembership = ChatMembership.Create(directChat.Id, companion.Id);
            membershipsRepository.AddChatMembership(ownerMembership);
            membershipsRepository.AddChatMembership(companionMembership);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new ChatResponse(
                directChat.Id,
                directChat.Name,
                directChat.Type,
                directChat.OwnerId,
                directChat.CreatedAt);
        }
    }
}
