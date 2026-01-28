using FluentValidation.TestHelper;
using Petrichor.Services.Gallery.Features.UpdateImageTags;

namespace Petrichor.Services.Gallery.Tests.Features.UpdateImageTags;

public class UpdateImageTagsCommandValidatorTests
{
    private readonly UpdateImageTagsCommandValidator _validator = new();

    [Fact]
    public void WithValidData_PassesValidation()
    {
        var command = new UpdateImageTagsCommand(Guid.NewGuid(), ["tag1", "tag2"]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WithInvalidData_ReturnsValidationError()
    {
        var command = new UpdateImageTagsCommand(Guid.NewGuid(), ["", " "]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Tags);
    }
}