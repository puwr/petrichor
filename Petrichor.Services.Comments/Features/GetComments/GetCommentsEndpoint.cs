using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared;
using Petrichor.Shared.Pagination;
using Wolverine;

namespace Petrichor.Services.Comments.Features.GetComments;

public class GetCommentsEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "comments",
            async (
                [FromQuery] Guid resourceId,
                [FromQuery] string? cursor,
                IMessageBus bus,
                CancellationToken cancellationToken) =>
            {
                var pagination = new CursorPaginationParameters(cursor);
                var query = new GetCommentsQuery(resourceId, pagination);

                var comments = await bus
                    .InvokeAsync<CursorPagedResponse<GetCommentsResponse>>(query, cancellationToken);

                return Results.Ok(comments);
            }
        )
        .WithTags(Tags.Comments)
        .WithSummary("Get comments")
        .Produces<CursorPagedResponse<GetCommentsResponse>>(StatusCodes.Status200OK);
    }
}