using System.Text.RegularExpressions;
using FluentValidation;

namespace Petrichor.Services.Users.Features.Authentication.Register;

public partial class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(c => c.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username is required.")
            .Matches(UsernameRegex).WithMessage(@"Username must be 3-30 characters long
                and only contain letters, digits or underscores.");

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(PasswordRegex).WithMessage(@"Password must be 8-128 characters long,
                have lowercase letter, uppercase letter,
                digit and special character.");
    }

    private static readonly Regex UsernameRegex = UsernameValidationRegex();
    private static readonly Regex PasswordRegex = PasswordValidationRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9_]{3,30}$")]
    private static partial Regex UsernameValidationRegex();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[A-Za-z\d\W]{8,128}$")]
    private static partial Regex PasswordValidationRegex();
}
