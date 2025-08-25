using ErrorOr;
using Petrichor.Modules.Gallery.Domain.Images;
using TestUtilities.Images;
using TestUtilities.Tags;

namespace Petrichor.Modules.Gallery.Domain.Tests.Images;

public class ImageTests
{
    [Fact]
    public void AddTags_AddsTagsToImage()
    {
        var image = ImageFactory.CreateImage();
        var tags = TagFactory.CreateTags();

        image.AddTags(tags);

        image.Tags.Should().BeEquivalentTo(tags);
    }

    [Fact]
    public void AddTags_WhenTagsDuplicate_IgnoresDuplicates()
    {
        var image = ImageFactory.CreateImage();
        var tags = TagFactory.CreateTags();

        image.AddTags(tags);
        image.AddTags(tags); // Duplicate tag

        image.Tags.Should().HaveCount(1);
    }

    [Fact]
    public void AddTags_WhenMaxTagsExceeded_ReturnsMaxTagsExceededError()
    {
        var image = ImageFactory.CreateImage();
        var tags = TagFactory.CreateTags(ImageConstants.MaxTagsPerImage + 1);

        var addTagsResult = image.AddTags(tags);

        addTagsResult.IsError.Should().BeTrue();
        addTagsResult.FirstError.Should().Be(ImageErrors.MaxTagsExceeded);
        image.Tags.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTag_RemovesTagFromImage()
    {
        var image = ImageFactory.CreateImage();
        var tag = TagFactory.CreateTags();

        image.AddTags(tag);

        var removeTagResult = image.RemoveTag(tag[0].Id);

        removeTagResult.IsError.Should().BeFalse();
        removeTagResult.Value.Should().Be(Result.Deleted);
        image.Tags.Should().NotContain(tag[0]);
        image.Tags.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTag_WhenTagNotAssociated_ReturnsTagNotAssociatedError()
    {
        var image = ImageFactory.CreateImage();

        var removeTagResult = image.RemoveTag(Guid.CreateVersion7());

        removeTagResult.IsError.Should().BeTrue();
        removeTagResult.FirstError.Should().Be(ImageErrors.TagNotAssociated);
    }
}