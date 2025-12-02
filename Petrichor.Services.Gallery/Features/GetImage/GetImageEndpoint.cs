using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.GetImage;

public class GetImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "images/{imageId:guid}",
            async (Guid imageId, IMessageBus bus) =>
            {
                var query = new GetImageQuery(imageId);

                var getImageResult = await bus.InvokeAsync<ErrorOr<GetImageResponse>>(query);

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