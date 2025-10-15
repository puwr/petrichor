using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Domain.Images;
using Petrichor.Services.Gallery.Common.Domain.Images.ValueObjects;

namespace Petrichor.Services.Gallery.Tests.Domain;

public class ImageTests
{
    [Fact]
    public void AddTags_AddsTagsToImage()
    {
        var image = CreateImage();
        var tags = CreateTags();

        image.AddTags(tags);

        image.Tags.Should().BeEquivalentTo(tags);
    }

    [Fact]
    public void AddTags_WhenTagsDuplicate_IgnoresDuplicates()
    {
        var image = CreateImage();
        var tags = CreateTags();

        image.AddTags(tags);
        image.AddTags(tags); // Duplicate tag

        image.Tags.Should().HaveCount(1);
    }

    [Fact]
    public void AddTags_WhenMaxTagsExceeded_ReturnsMaxTagsExceededError()
    {
        var image = CreateImage();
        var tags = CreateTags(ImageConstants.MaxTagsPerImage + 1);

        var addTagsResult = image.AddTags(tags);

        addTagsResult.IsError.Should().BeTrue();
        addTagsResult.FirstError.Should().Be(ImageErrors.MaxTagsExceeded);
        image.Tags.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTag_RemovesTagFromImage()
    {
        var image = CreateImage();
        var tag = CreateTags();

        image.AddTags(tag);

        image.RemoveTag(tag[0].Id);

        image.Tags.Should().NotContain(tag[0]);
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