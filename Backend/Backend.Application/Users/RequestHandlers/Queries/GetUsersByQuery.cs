using Backend.Application.Users.Interfaces;
using Backend.Application.Users.Models;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Users.RequestHandlers.Queries;

/// <summary>
/// Query to get users by name or email.
/// </summary>
public class GetUsersByQuery
{
    public record Query(string ClerkId, string Filter) : IRequest<IEnumerable<UpsertAppUser>>;

    public class Handler(
        IUsersRepository usersRepository)
        : IRequestHandler<Query, IEnumerable<UpsertAppUser>>
    {
        public async Task<IEnumerable<UpsertAppUser>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            (string clerkId, string filter) = request;

            IEnumerable<AppUser> users =
                await usersRepository.GetUsersByQueryAsync(clerkId, filter, cancellationToken);

            return users.Select(x =>
                new UpsertAppUser(x.ClerkId, x.Username, x.Email, x.ImageUrl));
        }
    }
}
