using ErrorOr;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public class DeleteImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete("images/{imageId:guid}", async (Guid imageId, IMessageBus bus) =>
        {
            var command = new DeleteImageCommand(imageId);

            var deleteImageResult = await bus.InvokeAsync<ErrorOr<Deleted>>(command);

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