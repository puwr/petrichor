using FluentValidation.TestHelper;
using Petrichor.Services.Users.Features.Authentication.Login;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();
    [Fact]
    public void WithValidCredentials_PassesValidation()
    {
        var command = new LoginCommand("test@example.com", "password");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void WithInvalidEmail_ReturnsValidationError(string email)
    {
        var command = new LoginCommand(email, "password");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void WithEmailTooLong_ReturnsValidationError()
    {
        var tooLongEmail = new string('a', 100) + "@example.com";
        var command = new LoginCommand(tooLongEmail, "password");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void WithEmptyPassword_ReturnsValidationError()
    {
        var command = new LoginCommand("test@example.com", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void WithPasswordTooLong_ReturnsValidationError()
    {
        var tooLongPassword = new string('a', 129);
        var command = new LoginCommand("test@example.com", tooLongPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}