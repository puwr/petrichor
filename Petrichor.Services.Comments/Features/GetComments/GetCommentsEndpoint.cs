using MediatR;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Comments.Features.GetComments;

public class GetCommentsEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "comments",
            async ([FromQuery] Guid resourceId, [FromQuery] string? cursor, ISender mediator) =>
            {
                var paginationParameters = new CursorPaginationParameters(cursor);
                var query = new GetCommentsQuery(resourceId, paginationParameters);

                var getCommentsResult = await mediator.Send(query);

                return getCommentsResult.Match(
                    Results.Ok,
                    Problem
                );
            }
        )
        .WithTags(Tags.Comments)
        .WithSummary("Get comments")
        .Produces<CursorPagedResponse<GetCommentsResponse>>(StatusCodes.Status200OK);
    }
}