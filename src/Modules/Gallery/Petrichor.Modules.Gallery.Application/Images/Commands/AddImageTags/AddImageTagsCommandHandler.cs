using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Utilities;
using Petrichor.Modules.Gallery.Domain.Tags;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.AddImageTags;

public class AddImageTagsCommandHandler(IGalleryDbContext dbContext)
    : IRequestHandler<AddImageTagsCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        AddImageTagsCommand command,
        CancellationToken cancellationToken)
    {
        var tags = await GetOrCreateTagsAsync(command.Tags, cancellationToken);

        if (tags.Count == 0)
        {
            return Error.Validation("No valid tags provided.");
        }

        var image = await dbContext.Images
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == command.ImageId,
                cancellationToken: cancellationToken);

        if (image is null)
        {
            return Error.NotFound("Image not found.");
        }

        var addTagsResult = image.AddTags(tags);

        if (addTagsResult.IsError)
        {
            return addTagsResult.Errors;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(
        List<string> tags,
        CancellationToken cancellationToken = default)
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
            .Select(t => new Tag(t))
            .ToList();

        return [.. existingTags, .. newTags];
    }
}