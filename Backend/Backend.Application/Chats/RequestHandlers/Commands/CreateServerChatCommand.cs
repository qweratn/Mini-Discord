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
/// Command to create a new server chat.
/// </summary>
public class CreateServerChatCommand
{
    public record Command(string Name, string ClerkId) : IRequest<ChatResponse>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<Command, ChatResponse>
    {
        public async Task<ChatResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            (string name, string clerkId) = request;

            AppUser owner = await usersRepository.GetUserByClerkIdAsync(clerkId, cancellationToken)
                            ?? throw new NotFoundException(
                                "user.not_found",
                                "User was not found.");

            Chat chat = Chat.CreateServer(name, owner.Id);
            chatsRepository.AddChat(chat);

            ChatMembership chatMembership = ChatMembership.Create(chat.Id, owner.Id);
            membershipsRepository.AddChatMembership(chatMembership);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ChatResponse(
                chat.Id,
                chat.Name,
                chat.Type,
                chat.OwnerId,
                chat.CreatedAt);
        }
    }
}
