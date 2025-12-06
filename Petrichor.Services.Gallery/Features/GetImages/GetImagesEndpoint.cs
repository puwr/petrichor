using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared;
using Petrichor.Shared.Pagination;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.GetImages;

public class GetImagesEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "images",
            async (
                IMessageBus bus,
                CancellationToken cancellationToken,
                [FromQuery(Name = "page")] int pageNumber = 1,
                [FromQuery] string[]? tags = null,
                [FromQuery] string? uploader = null) =>
            {
                var pagination = new PaginationParameters(pageNumber, PageSize: 15);

                var query = new GetImagesQuery(
                    Pagination: pagination,
                    Tags: tags,
                    Uploader: uploader?.ToLowerInvariant());

                var images = await bus
                    .InvokeAsync<PagedResponse<GetImagesResponse>>(query, cancellationToken);

                return Results.Ok(images);
            })
            .WithTags(Tags.Images)
            .WithSummary("Get images")
            .Produces<PagedResponse<GetImagesResponse>>(StatusCodes.Status200OK);
    }
}