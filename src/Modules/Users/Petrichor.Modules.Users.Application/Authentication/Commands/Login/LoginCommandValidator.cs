using FluentValidation;
using Petrichor.Shared.Application.Common;

namespace Petrichor.Modules.Users.Application.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage(ValidationMessages.InvalidCredentials)
            .EmailAddress().WithMessage(ValidationMessages.InvalidCredentials)
            .MaximumLength(100).WithMessage(ValidationMessages.InvalidCredentials);

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage(ValidationMessages.InvalidCredentials)
            .MaximumLength(128).WithMessage(ValidationMessages.InvalidCredentials);
    }
}
