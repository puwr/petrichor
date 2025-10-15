using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Gallery.Features.GetImage;

public class GetImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("images/{imageId:guid}", async (Guid imageId, ISender mediator) =>
        {
            var query = new GetImageQuery(imageId);

            var getImageResult = await mediator.Send(query);

            return getImageResult.Match(
                Results.Ok,
                Problem
            );
        })
        .WithTags(Tags.Images)
        .WithSummary("Get image")
        .Produces<GetImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}