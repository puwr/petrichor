using FluentAssertions;
using TestUtils.Images;

namespace Domain.UnitTests.Images;

public class ImageTests
{
    [Fact]
    public void RemoveTag_WhenTagDoesntExist_ShouldFail()
    {
        var image = ImageFactory.CreateImage();
        
        var removeTagResult = image.RemoveTag(Guid.NewGuid());

        removeTagResult.IsError.Should().BeTrue();
    }
}