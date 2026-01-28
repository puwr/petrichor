using FluentValidation.TestHelper;
using Petrichor.Services.Users.Features.Authentication.Register;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();
    private const string ValidEmail = "test@example.com";
    private const string ValidUserName = "test";
    private const string ValidPassword = "Pa$$w0rd";

    [Fact]
    public void WithValidCredentials_PassesValidation()
    {
        var command = new RegisterCommand(ValidEmail, ValidUserName, ValidPassword);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void WithInvalidEmail_ReturnsValidationError(string email)
    {
        var command = new RegisterCommand(email, ValidUserName, ValidPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void WithEmailTooLong_ReturnsValidationError()
    {
        var tooLongEmail = new string('t', 100) + "@example.com";
        var command = new RegisterCommand(tooLongEmail, ValidUserName, ValidPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("username?")]
    [InlineData("t")]
    public void WithInvalidUserName_ReturnsValidationError(string userName)
    {
        var command = new RegisterCommand(ValidEmail, userName, ValidPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserName);
    }

    [Fact]
    public void WithUserNameTooLong_ReturnsValidationError()
    {
        var tooLongUserName = new string('t', 31);
        var command = new RegisterCommand(ValidEmail, tooLongUserName, ValidPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UserName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("pass")]
    [InlineData("password")]
    public void WithInvalidPassword_ReturnsValidationError(string password)
    {
        var command = new RegisterCommand(ValidEmail, ValidUserName, password);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void WithPasswordTooLong_ReturnsValidationError()
    {
        var tooLongPassword = new string('t', 31);
        var command = new RegisterCommand(ValidEmail, ValidUserName, tooLongPassword);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}