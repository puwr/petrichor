using FluentValidation;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private const string InvalidCredentials = "Invalid credentials.";

    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(InvalidCredentials)
            .EmailAddress().WithMessage(InvalidCredentials)
            .MaximumLength(100).WithMessage(InvalidCredentials);

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(InvalidCredentials)
            .MaximumLength(128).WithMessage(InvalidCredentials);
    }
}
