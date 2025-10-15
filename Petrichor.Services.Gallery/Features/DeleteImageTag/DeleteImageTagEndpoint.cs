using MediatR;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Gallery.Features.DeleteImageTag;

public class DeleteImageTagEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete("images/{imageId:guid}/tags/{tagId:guid}", async (
            Guid imageId,
            Guid tagId,
            ISender mediator) =>
        {
            var command = new DeleteImageTagCommand(imageId, tagId);

            var deleteImageTagResult = await mediator.Send(command);

            return deleteImageTagResult.Match(
                _ => Results.NoContent(),
                Problem);
        })
        .RequireAuthorization(GalleryPolicies.ImageUploaderOrAdmin)
        .WithTags(Tags.Images)
        .WithSummary("Delete tag from image")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}