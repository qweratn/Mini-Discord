using Backend.Application.Chats.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for <see cref="JoinChatCommand"/>.
/// </summary>
public class JoinChatCommandValidator
    : AbstractValidator<JoinChatCommand.Command>
{
    public JoinChatCommandValidator()
    {
        RuleFor(x => x.ClerkId)
            .NotNull()
            .WithMessage("ClerkId cannot be null");

        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("ChatId cannot be empty");
    }
}
