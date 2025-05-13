using Domain.Images;
using ErrorOr;
using FluentAssertions;
using TestUtils.Images;

namespace Domain.UnitTests.Images;

public class ImageTests
{
    [Fact]
    public void AddTag_WhenTagAlreadyExists_ShouldFail()
    {
        var image = ImageFactory.CreateImage();
        var tag = TagFactory.CreateTag();
        var addTagResults = Enumerable.Range(0, 2)
            .Select(_ => image.AddTag(tag)).ToList();

        addTagResults.First().Value.Should().Be(Result.Success);

        addTagResults.Last().IsError.Should().BeTrue();
        addTagResults.Last().FirstError.Should().Be(ImageErrors.TagAlreadyAdded);
    }
}