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
    private const int MaxServerNameLength = 64;

    public CreateServerChatCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(MaxServerNameLength)
            .WithMessage($"Name cannot exceed {MaxServerNameLength} characters");

        RuleFor(x => x.ClerkId)
            .NotEmpty();
    }
}
