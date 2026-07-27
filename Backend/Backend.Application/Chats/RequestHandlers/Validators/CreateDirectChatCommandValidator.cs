using Backend.Application.Chats.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

public class CreateDirectChatCommandValidator
    : AbstractValidator<CreateDirectChatCommand.Command>
{
    public CreateDirectChatCommandValidator()
    {
        RuleFor(x => x.ClerkId)
            .NotEmpty();

        RuleFor(x => x.CompanionUserId)
            .NotEmpty()
            .WithMessage("Companion user ID cannot be empty");
    }
}
