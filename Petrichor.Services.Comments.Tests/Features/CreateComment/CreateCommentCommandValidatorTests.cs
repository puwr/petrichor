using FluentValidation.TestHelper;
using Petrichor.Services.Comments.Features.CreateComment;

namespace Petrichor.Services.Comments.Tests.Features.CreateComment;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator = new();

    [Fact]
    public void WithValidData_PassesValidation()
    {
        var command = new CreateCommentCommand(
            AuthorId: Guid.NewGuid(),
            ResourceId: Guid.NewGuid(),
            Message: "Test");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void WithInvalidMessage_ReturnsValidationError(string message)
    {
        var command = new CreateCommentCommand(
            AuthorId: Guid.NewGuid(),
            ResourceId: Guid.NewGuid(),
            Message: message);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Message);
    }

    [Fact]
    public void WithMessageTooLong_ReturnsValidationError()
    {
        var command = new CreateCommentCommand(
            AuthorId: Guid.NewGuid(),
            ResourceId: Guid.NewGuid(),
            Message: new string('t', 1001));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Message);
    }
}