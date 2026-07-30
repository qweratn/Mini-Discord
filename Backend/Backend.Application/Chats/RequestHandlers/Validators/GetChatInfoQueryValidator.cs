using Backend.Application.Chats.RequestHandlers.Queries;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for
/// <see cref="GetChatInfoQuery"/>.
/// </summary>
public class GetChatInfoQueryValidator
    : AbstractValidator<GetChatInfoQuery.Query>
{
    public GetChatInfoQueryValidator()
    {
        RuleFor(x => x.ChatId)
            .NotNull()
            .NotEmpty()
            .WithMessage("ChatId cannot be null");
    }
}
