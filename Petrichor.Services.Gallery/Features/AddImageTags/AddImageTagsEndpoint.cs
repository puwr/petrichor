using MediatR;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Gallery.Features.AddImageTags;

public class AddImageTagsEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("images/{imageId:guid}/tags", async (
            Guid imageId,
            AddImageTagsRequest request,
            ISender mediator) =>
        {
            var command = new AddImageTagsCommand(imageId, request.Tags);

            var addImageTagResult = await mediator.Send(command);

            return addImageTagResult.Match(
                _ => Results.NoContent(),
                Problem
            );
        })
        .RequireAuthorization(GalleryPolicies.ImageUploaderOrAdmin)
        .WithTags(Tags.Images)
        .WithSummary("Add tags to image")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}