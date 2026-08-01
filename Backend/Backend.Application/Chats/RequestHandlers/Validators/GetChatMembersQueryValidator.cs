using Backend.Application.Chats.RequestHandlers.Queries;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for
/// <see cref="GetChatMembersQuery"/>.
/// </summary>
public class GetChatMembersQueryValidator
    : AbstractValidator<GetChatMembersQuery.Query>
{
    public GetChatMembersQueryValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("ChatId cannot be empty.");
    }
}
