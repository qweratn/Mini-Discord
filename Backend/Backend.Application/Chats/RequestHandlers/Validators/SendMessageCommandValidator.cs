using Backend.Application.Chats.RequestHandlers.Commands;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

public class SendMessageCommandValidator
    : AbstractValidator<SendMessageCommand.Command>
{
    private const int MaxContentLength = 2000;

    public SendMessageCommandValidator()
    {
        RuleFor(x => x.SendMessage.Content)
            .NotEmpty()
            .WithMessage("Content cannot be empty")
            .MaximumLength(MaxContentLength)
            .WithMessage($"Content cannot be longer than {MaxContentLength} characters");

        RuleFor(x => x.SendMessage.ChatId)
            .NotEmpty()
            .WithMessage("ChatId cannot be empty");

        RuleFor(x => x.SendMessage.ChatId)
            .NotEmpty()
            .WithMessage("ChatId cannot be empty");
    }
}
