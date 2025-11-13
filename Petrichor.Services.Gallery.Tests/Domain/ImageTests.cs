using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Domain.Images;
using Petrichor.Services.Gallery.Common.Domain.Images.ValueObjects;

namespace Petrichor.Services.Gallery.Tests.Domain;

public class ImageTests
{
    [Fact]
    public void UpdateTags_UpdatesImageTags()
    {
        var image = CreateImage();
        var tags = CreateTags();

        image.UpdateTags(tags);
        image.Tags.Should().BeEquivalentTo(tags);

        var newTags = CreateTags(count: 2);
        image.UpdateTags(newTags);
        image.Tags.Should().BeEquivalentTo(newTags);
    }

    [Fact]
    public void UpdateTags_WhenMaxTagsExceeded_ReturnsMaxTagsExceededError()
    {
        var image = CreateImage();
        var tags = CreateTags(ImageConstants.MaxTagsPerImage + 1);

        var updateTagsResult = image.UpdateTags(tags);

        updateTagsResult.IsError.Should().BeTrue();
        updateTagsResult.FirstError.Should().Be(ImageErrors.MaxTagsExceeded);
        image.Tags.Should().BeEmpty();
    }

    private static Image CreateImage()
    {
        return Image.Create(
            originalImage: new OriginalImage(
                Path: "/uploads/original.jpg",
                Width: 2000,
                Height: 1500),
            thumbnail: new Thumbnail(
                Path: "/thumbs/thumb.jpg",
                Width: 300,
                Height: 200),
            uploaderId: Guid.Parse("9eb737a7-7483-465c-8284-785c1b59436b")
        );
    }

    private static List<Tag> CreateTags(int count = 1)
    {
        return Enumerable.Range(1, count)
            .Select(i => Tag.Create($"tag{i}"))
            .ToList();
    }
}