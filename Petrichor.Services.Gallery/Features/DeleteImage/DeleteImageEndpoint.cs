using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public class DeleteImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "images/{imageId:guid}",
            async (Guid imageId, IMessageBus bus, CancellationToken cancellationToken) =>
            {
                var command = new DeleteImageCommand(imageId);

                await bus.InvokeAsync(command, cancellationToken);

                return Results.NoContent();
            })
            .RequireAuthorization(GalleryPolicies.ImageUploaderOrAdmin)
            .WithTags(Tags.Images)
            .WithSummary("Delete image")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}