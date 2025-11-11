using MediatR;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Gallery.Features.GetImages;

public class GetImagesEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "images",
            async (
                ISender mediator,
                [FromQuery(Name = "page")] int pageNumber = 1,
                [FromQuery] string[]? tags = null,
                [FromQuery] string? uploader = null) =>
            {
                var pagination = new PaginationParameters(pageNumber, PageSize: 20);

                var query = new GetImagesQuery(
                    Pagination: pagination,
                    Tags: tags?.ToList() ?? null,
                    Uploader: uploader?.ToLowerInvariant());

                var getImagesResult = await mediator.Send(query);

                return getImagesResult.Match(
                    Results.Ok,
                    Problem
                );
            })
            .WithTags(Tags.Images)
            .WithSummary("Get images")
            .Produces<PagedResponse<GetImagesResponse>>(StatusCodes.Status200OK);
    }
}