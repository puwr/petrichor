using System.Text.RegularExpressions;
using Domain.Users;
using FluentValidation;

namespace Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username is required.")
            .Matches(UsernameRegex).WithMessage(@"Username must be 3-30 characters long
                and only contain letters, digits or underscores.");

        RuleFor(c => c.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(PasswordRegex).WithMessage(@"Password must be 8-128 characters long,
                have lowercase letter, uppercase letter,
                digit and special character.");
    }

    private static readonly Regex UsernameRegex = new(
        pattern: """
            ^
            [a-zA-Z0-9_]{3,30} // letters, digits, underscores, 3-30 chars
            $
        """,
        options: RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );

    private static readonly Regex PasswordRegex = new(
        pattern: """
            ^
            (?=.*[a-z]) // lowercase letter
            (?=.*[A-Z]) // uppercase letter
            (?=.*\\d) // digit
            (?=.*\\W) // non-alphanumeric char
            [A-Za-z\\d\\W]{8,128} // 8-128 chars
            $'
        """,
        options: RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );
}
