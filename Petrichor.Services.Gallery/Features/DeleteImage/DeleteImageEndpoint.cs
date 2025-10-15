using MediatR;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public class DeleteImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete("images/{imageId:guid}", async (Guid imageId, ISender mediator) =>
        {
            var command = new DeleteImageCommand(imageId);

            var deleteImageResult = await mediator.Send(command);

            return deleteImageResult.Match(
                _ => Results.NoContent(),
                Problem);
        })
        .RequireAuthorization(GalleryPolicies.ImageUploaderOrAdmin)
        .WithTags(Tags.Images)
        .WithSummary("Delete image")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}