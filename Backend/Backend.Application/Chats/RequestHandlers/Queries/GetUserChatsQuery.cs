using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Common.Exceptions;
using Backend.Application.Messages.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Enums;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Queries;

/// <summary>
/// Query to get all chats available to the current user.
/// </summary>
public class GetUserChatsQuery
{
    public record Query(string ClerkId) : IRequest<IReadOnlyList<UserChatListItem>>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository membershipsRepository,
        IMessagesRepository messagesRepository)
        : IRequestHandler<Query, IReadOnlyList<UserChatListItem>>
    {
        public async Task<IReadOnlyList<UserChatListItem>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            AppUser user = await usersRepository.GetUserByClerkIdAsync(
                               request.ClerkId,
                               cancellationToken)
                           ?? throw new NotFoundException(
                               "user.not_found",
                               "User was not found.");

            IReadOnlyList<Chat> userChats = await chatsRepository.GetUserChatsAsync(
                user.Id,
                cancellationToken);

            Guid[] chatIds = userChats
                .Select(chat => chat.Id)
                .ToArray();

            IReadOnlyDictionary<Guid, Message> messages =
                await messagesRepository.GetLastMessagesAsync(
                    chatIds,
                    cancellationToken);

            Chat[] directChats = userChats
                .Where(chat => chat.Type == ChatType.Direct)
                .ToArray();

            Guid[] directChatIds = directChats
                .Select(chat => chat.Id)
                .ToArray();

            IReadOnlyList<ChatMembership> directChatMemberships =
                await membershipsRepository.GetByChatIdsAsync(
                    directChatIds,
                    cancellationToken);

            IReadOnlyDictionary<Guid, Guid> companionIdsByChatId =
                GetCompanionIdsByChatId(
                    user.Id,
                    directChats,
                    directChatMemberships);

            IReadOnlyDictionary<Guid, AppUser> companions =
                await usersRepository.GetUsersByIdsAsync(
                    companionIdsByChatId.Values.Distinct(),
                    cancellationToken);

            return userChats
                .OrderByDescending(chat => GetLastActivityAt(chat, messages))
                .ThenByDescending(chat => chat.Id)
                .Select(chat => CreateChatResponse(
                    chat,
                    messages,
                    companionIdsByChatId,
                    companions))
                .ToList();
        }

        private static IReadOnlyDictionary<Guid, Guid> GetCompanionIdsByChatId(
            Guid currentUserId,
            IEnumerable<Chat> directChats,
            IEnumerable<ChatMembership> memberships)
        {
            ILookup<Guid, Guid> memberIdsByChatId = memberships
                .ToLookup(
                    membership => membership.ChatId,
                    membership => membership.MemberId);

            Dictionary<Guid, Guid> companionIdsByChatId = [];

            foreach (Chat chat in directChats)
            {
                Guid[] memberIds = memberIdsByChatId[chat.Id]
                    .Distinct()
                    .ToArray();

                if (memberIds.Length != 2 ||
                    !memberIds.Contains(currentUserId))
                {
                    throw new ConflictException(
                        "chat.direct_members_invalid",
                        "Direct chat must contain the current user and exactly one companion.");
                }

                companionIdsByChatId[chat.Id] =
                    memberIds.Single(memberId => memberId != currentUserId);
            }

            return companionIdsByChatId;
        }

        private static DateTimeOffset GetLastActivityAt(
            Chat chat,
            IReadOnlyDictionary<Guid, Message> messages)
        {
            return messages.TryGetValue(chat.Id, out Message? message)
                ? message.SendAt
                : chat.CreatedAt;
        }

        private static UserChatListItem CreateChatResponse(
            Chat chat,
            IReadOnlyDictionary<Guid, Message> messages,
            IReadOnlyDictionary<Guid, Guid> companionIdsByChatId,
            IReadOnlyDictionary<Guid, AppUser> companions)
        {
            messages.TryGetValue(chat.Id, out Message? message);

            if (chat.Type == ChatType.Server)
            {
                string chatName = chat.Name
                                  ?? throw new ConflictException(
                                      "chat.server_name_missing",
                                      "Server chat name is missing.");

                return CreateResponse(
                    chatId: chat.Id,
                    chatName: chatName,
                    chatType: chat.Type,
                    imageUrl: null,
                    message: message);
            }

            if (!companionIdsByChatId.TryGetValue(
                    chat.Id,
                    out Guid companionId))
            {
                throw new ConflictException(
                    "chat.direct_companion_missing",
                    "Direct chat companion is missing.");
            }

            if (!companions.TryGetValue(companionId, out AppUser? companion))
            {
                throw new NotFoundException(
                    "user.not_found",
                    "Companion was not found.");
            }

            return CreateResponse(
                chatId: chat.Id,
                chatName: companion.Username,
                chatType: chat.Type,
                imageUrl: companion.ImageUrl,
                message: message);
        }

        private static UserChatListItem CreateResponse(
            Guid chatId,
            string chatName,
            ChatType chatType,
            string? imageUrl,
            Message? message)
        {
            return new UserChatListItem(
                chatId,
                chatName,
                chatType,
                imageUrl,
                message?.Content,
                message?.SendAt);
        }
    }
}
