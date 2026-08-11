using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Common.Exceptions;
using Backend.Application.Messages.Interfaces;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Chats;
using Backend.Domain.Messages;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Queries;

/// <summary>
/// Query to get chat messages.
/// </summary>
public class GetChatMessagesQuery
{
    private const int PageSize = 50;

    public record Query(
        string ClerkId,
        Guid ChatId,
        Guid? BeforeMessageId) : IRequest<GetChatMessageResponse>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IChatMembershipsRepository chatMembershipsRepository,
        IMessagesRepository messagesRepository)
        : IRequestHandler<Query, GetChatMessageResponse>
    {
        public async Task<GetChatMessageResponse> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            (string clerkId, Guid chatId, Guid? beforeMessageId) = request;

            AppUser user = await usersRepository.GetUserByClerkIdAsync(clerkId, cancellationToken) ??
                           throw new NotFoundException(
                               "user.not_found",
                               "User was not found.");

            Chat chat = await chatsRepository.GetChatByIdAsync(chatId, cancellationToken) ??
                        throw new NotFoundException(
                            "chat.not_found",
                            "Chat was not found.");

            await CheckMembershipAsync(user.Id, chat.Id, cancellationToken);

            Message? cursorMessage = await GetCursorMessageAsync(beforeMessageId, chat.Id, cancellationToken);

            IReadOnlyList<Message> messages = await messagesRepository
                .GetChatMessagesAsync(
                    chat.Id,
                    cursorMessage?.SendAt,
                    cursorMessage?.Id,
                    PageSize + 1,
                    cancellationToken);

            List<Message> page = messages
                .Take(PageSize)
                .ToList();

            List<ChatMessageResponse> items = await GetChatMessageResponsesAsync(page, cancellationToken);

            return CreateResponse(messages, page, items);
        }

        private async Task CheckMembershipAsync(
            Guid userId,
            Guid chatId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<ChatMembership> memberships =
                await chatMembershipsRepository.GetByChatIdsAsync([chatId], cancellationToken);

            bool currentUserIsMember = memberships.Any(
                membership => membership.MemberId == userId);

            if (!currentUserIsMember)
            {
                throw new ForbiddenException(
                    "chat.access_denied",
                    "User is not a member of this chat.");
            }
        }

        private async Task<Message?> GetCursorMessageAsync(
            Guid? beforeMessageId,
            Guid chatId,
            CancellationToken cancellationToken)
        {
            Message? cursorMessage = null;

            if (beforeMessageId.HasValue)
            {
                cursorMessage = await messagesRepository.GetMessageByIdAsync(
                    beforeMessageId.Value,
                    cancellationToken);

                if (cursorMessage is null ||
                    cursorMessage.ChatId != chatId)
                {
                    throw new ArgumentException(
                        "Message cursor is invalid.");
                }
            }

            return cursorMessage;
        }

        private async Task<List<ChatMessageResponse>> GetChatMessageResponsesAsync(
            List<Message> page,
            CancellationToken cancellationToken)
        {
            Guid[] authorIds = page
                .Select(message => message.AuthorId)
                .Distinct()
                .ToArray();

            IReadOnlyDictionary<Guid, AppUser> authors =
                await usersRepository.GetUsersByIdsAsync(
                    authorIds,
                    cancellationToken);

            return page
                .Select(message =>
                {
                    if (!authors.TryGetValue(
                            message.AuthorId,
                            out AppUser? author))
                    {
                        throw new NotFoundException(
                            "user.not_found",
                            "Message author was not found.");
                    }

                    return new ChatMessageResponse(
                        message.Id,
                        message.ChatId,
                        message.Content,
                        new MessageAuthorResponse(
                            author.Id,
                            author.Username,
                            author.ImageUrl),
                        message.SendAt);
                })
                .ToList();
        }

        private GetChatMessageResponse CreateResponse(
            IReadOnlyList<Message> messages,
            List<Message> page,
            List<ChatMessageResponse> items)
        {
            bool hasMore = messages.Count > PageSize;

            Guid? nextBeforeMessageId =
                hasMore && page.Count > 0
                    ? page[^1].Id
                    : null;

            return new GetChatMessageResponse(
                items,
                nextBeforeMessageId,
                hasMore);
        }
    }
}
