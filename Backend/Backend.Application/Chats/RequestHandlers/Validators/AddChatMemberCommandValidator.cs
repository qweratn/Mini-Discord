using Backend.Application.Chats.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for <see cref="AddChatMemberCommand"/>.
/// </summary>
public class AddChatMemberCommandValidator
    : AbstractValidator<AddChatMemberCommand.Command>
{
    public AddChatMemberCommandValidator()
    {
        RuleFor(x => x.ActorUserId)
            .NotEmpty()
            .WithMessage("ActorUserId cannot be empty.");

        RuleFor(x => x.TargetUserId)
            .NotEmpty()
            .WithMessage("TargetUserId cannot be empty.");

        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("ChatId cannot be empty.");
    }
}
