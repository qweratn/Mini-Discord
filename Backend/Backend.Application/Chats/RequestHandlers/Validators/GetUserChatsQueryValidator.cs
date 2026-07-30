using Backend.Application.Chats.RequestHandlers.Queries;
using FluentValidation;

namespace Backend.Application.Chats.RequestHandlers.Validators;

/// <summary>
/// Validator for <see cref="GetUserChatsQuery.Query"/>.
/// </summary>
public class GetUserChatsQueryValidator
    : AbstractValidator<GetUserChatsQuery.Query>
{
    public GetUserChatsQueryValidator()
    {
        RuleFor(query => query.ClerkId)
            .NotEmpty()
            .WithMessage("Clerk ID is required.");
    }
}
