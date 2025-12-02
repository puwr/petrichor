using ErrorOr;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.UpdateImageTags;

public class UpdateImageTagsEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPatch("images/{imageId:guid}/tags", async (
            Guid imageId,
            UpdateImageTagsRequest request,
            IMessageBus bus) =>
        {
            var command = new UpdateImageTagsCommand(imageId, request.Tags);

            var addImageTagResult = await bus.InvokeAsync<ErrorOr<Success>>(command);

            return addImageTagResult.Match(
                _ => Results.NoContent(),
                Problem
            );
        })
        .RequireAuthorization(GalleryPolicies.ImageUploaderOrAdmin)
        .WithTags(Tags.Images)
        .WithSummary("Update image tags")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}