using Backend.Application.Users.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Users.RequestHandlers.Validators;

/// <summary>
/// Validators for
/// <see cref="SyncUserFromClerkCommand"/>.
/// </summary>
public class SyncUserFromClerkCommandValidator
    : AbstractValidator<SyncUserFromClerkCommand.Command>
{
    public SyncUserFromClerkCommandValidator()
    {
        RuleFor(x => x.AppUser.ClerkId)
            .NotEmpty()
            .WithMessage("Clerk ID is required.");

        RuleFor(x => x.AppUser.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(32)
            .WithMessage("Username cannot exceed 32 characters.");

        RuleFor(x => x.AppUser.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email address is invalid.");

        RuleFor(x => x.AppUser.ImageUrl)
            .NotEmpty()
            .WithMessage("Image URL is required.");
    }
}
