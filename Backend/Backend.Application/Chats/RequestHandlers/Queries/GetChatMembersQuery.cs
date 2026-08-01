using Backend.Application.ChatMemberships.Interfaces;
using Backend.Application.Chats.Models.Responses;
using Backend.Application.Users.Interfaces;
using Backend.Domain.ChatMemberships;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Chats.RequestHandlers.Queries;

/// <summary>
/// Query to get all chat members.
/// </summary>
public class GetChatMembersQuery
{
    public record Query(Guid ChatId) : IRequest<IReadOnlyList<ChatMemberInfo>>;

    public class Handler(
        IUsersRepository usersRepository,
        IChatMembershipsRepository membershipsRepository)
        : IRequestHandler<Query, IReadOnlyList<ChatMemberInfo>>
    {
        public async Task<IReadOnlyList<ChatMemberInfo>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<ChatMembership> memberships =
                await membershipsRepository.GetByChatIdsAsync([request.ChatId], cancellationToken);

            IReadOnlyList<Guid> memberIds = memberships
                .Select(membership => membership.MemberId)
                .ToList();

            Dictionary<Guid, AppUser> members = await usersRepository
                    .GetUsersByIdsAsync(memberIds, cancellationToken);

            return members.Values
                .Select(m => new ChatMemberInfo(
                    m.Id,
                    m.Username,
                    m.Email,
                    m.ImageUrl))
                .ToList();
        }
    }
}
