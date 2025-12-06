using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Utilities;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Gallery.Features.UpdateImageTags;

public class UpdateImageTagsCommandHandler(GalleryDbContext dbContext, IFusionCache cache)
{
    public async Task<ErrorOr<Success>> Handle(
        UpdateImageTagsCommand command,
        CancellationToken cancellationToken)
    {
        var image = await dbContext.Images
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == command.ImageId,
                cancellationToken: cancellationToken);

        if (image is null)
        {
            return Error.NotFound(description: "Image not found.");
        }

        var tags = await GetOrCreateTagsAsync(command.Tags, cancellationToken);

        var addTagsResult = image.UpdateTags(tags);

        if (addTagsResult.IsError)
        {
            return addTagsResult.Errors;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.ExpireAsync($"image:{image.Id}", token: cancellationToken);

        return Result.Success;
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(
        List<string> tags,
        CancellationToken cancellationToken)
    {
        var normalizedTagNames = TagHelpers.Normalize(tags);

        if (normalizedTagNames.Count == 0)
        {
            return [];
        }

        var existingTags = await dbContext.Tags
            .Where(t => normalizedTagNames.Contains(t.Name))
            .ToListAsync(cancellationToken: cancellationToken);

        var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();

        var newTags = normalizedTagNames
            .Where(t => !existingTagNames.Contains(t))
            .Select(t => Tag.Create(t))
            .ToList();

        return [.. existingTags, .. newTags];
    }
}