using Backend.Application.Common.Interfaces;
using Backend.Application.Users.RequestHandlers.Interfaces;
using Backend.Domain.Users;
using MediatR;

namespace Backend.Application.Users.RequestHandlers.Commands;

/// <summary>
/// Command to synchronize user information from Clerk.
/// </summary>
public class SyncUserFromClerkCommand
{
    public record Command(
        string ClerkId,
        string Username,
        string Email) : IRequest;

    public class Handler(IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            AppUser? user = await usersRepository.GetUserByClerkIdAsync(request.ClerkId);

            if (user == null)
            {
                AppUser newUser = AppUser.SyncFromClerk(
                    request.ClerkId,
                    request.Username,
                    request.Email);

                usersRepository.AddUser(newUser);
            }
            else
            {
                user.SyncProfile(
                    request.Username,
                    request.Email);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
