using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Common.Exceptions;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Queries;

/// <summary>
/// Query to get chat info.
/// </summary>
public class GetChatInfoQuery
{
    public record Query(string ClerkId, Guid ChatId) : IRequest<ChatInfo>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository chatMembershipsRepository)
        : IRequestHandler<Query, ChatInfo>
    {
        public async Task<ChatInfo> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            (string clerkId, Guid chatId) = request;

            AppUser user = await usersRepository.GetUserByClerkIdAsync(clerkId, cancellationToken) ??
                           throw new NotFoundException(
                               "user.not_found",
                               "User was not found.");

            Chat chat = await chatsRepository.GetChatInfoAsync(chatId, cancellationToken) ??
                   throw new NotFoundException(
                       "chat.not_found",
                       "Chat was not found");

            IReadOnlyList<ChatMembership> memberships =
                await chatMembershipsRepository.GetByChatIdsAsync([chatId], cancellationToken);

            bool currentUserIsMember = memberships.Any(
                membership => membership.MemberId == user.Id);

            if (!currentUserIsMember)
            {
                throw new ForbiddenException(
                    "chat.access_denied",
                    "User is not a member of this chat.");
            }

            return chat.Type switch
            {
                ChatType.Server => CreateServerChatInfo(chat, memberships.Count),

                ChatType.Direct => await CreateDirectChatInfoAsync(
                    chat,
                    user.Id,
                    memberships,
                    cancellationToken),

                _ => throw new ConflictException(
                    "chat.type_invalid",
                    "Chat type is invalid."),
            };
        }

        private ChatInfo CreateServerChatInfo(
            Chat chat,
            int membersCount)
        {
            string name = chat.Name
                          ?? throw new ConflictException(
                              "chat.server_name_missing",
                              "Server chat name is missing.");

            return new ChatInfo(
                chat.Id,
                name,
                chat.Type,
                ImageUrl: null,
                membersCount);
        }

        private async Task<ChatInfo> CreateDirectChatInfoAsync(
            Chat chat,
            Guid currentUserId,
            IReadOnlyCollection<ChatMembership> memberships,
            CancellationToken cancellationToken)
        {
            Guid[] memberIds = memberships
                .Select(membership => membership.MemberId)
                .Distinct()
                .ToArray();

            if (memberIds.Length != 2 ||
                !memberIds.Contains(currentUserId))
            {
                throw new ConflictException(
                    "chat.direct_members_invalid",
                    "Direct chat must contain the current user and exactly one companion.");
            }

            Guid companionId = memberIds.Single(
                memberId => memberId != currentUserId);

            AppUser companion = await usersRepository.GetUserByIdAsync(
                                    companionId,
                                    cancellationToken)
                                ?? throw new NotFoundException(
                                    "user.not_found",
                                    "Companion was not found.");

            return new ChatInfo(
                chat.Id,
                companion.Username,
                chat.Type,
                companion.ImageUrl,
                memberIds.Length);
        }
    }
}
