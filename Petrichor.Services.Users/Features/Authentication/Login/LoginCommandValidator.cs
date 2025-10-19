using FluentValidation;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private const string InvalidCredentials = "Invalid credentials.";

    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage(InvalidCredentials)
            .EmailAddress().WithMessage(InvalidCredentials)
            .MaximumLength(100).WithMessage(InvalidCredentials);

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage(InvalidCredentials)
            .MaximumLength(128).WithMessage(InvalidCredentials);
    }
}
