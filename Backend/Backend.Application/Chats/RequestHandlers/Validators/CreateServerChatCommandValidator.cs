using Backend.Application.Chats.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for
/// <see cref="CreateServerChatCommand"/>.
/// </summary>
public class CreateServerChatCommandValidator
    : AbstractValidator<CreateServerChatCommand.Command>
{
    public CreateServerChatCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("Name cannot exceed 64 characters");

        RuleFor(x => x.ClerkId)
            .NotEmpty();
    }
}
