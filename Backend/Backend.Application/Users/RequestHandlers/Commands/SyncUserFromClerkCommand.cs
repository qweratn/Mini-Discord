using Backend.Application.Common.Interfaces;
using Backend.Application.Users.Models;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Users.RequestHandlers.Commands;

/// <summary>
/// Command to synchronize user information from Clerk.
/// </summary>
public class SyncUserFromClerkCommand
{
    public record Command(UpsertAppUser AppUser) : IRequest;

    public class Handler(IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            (string clerkId, string username, string email, string imageUrl) = request.AppUser;
            AppUser? user = await usersRepository.GetUserByClerkIdAsync(clerkId);

            if (user == null)
            {
                AppUser newUser = AppUser.SyncFromClerk(
                    clerkId,
                    username,
                    email,
                    imageUrl);

                usersRepository.AddUser(newUser);
            }
            else
            {
                user.SyncProfile(
                    username,
                    email,
                    imageUrl);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
