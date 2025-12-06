using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Shared;
using Petrichor.Shared.Pagination;
using Wolverine;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

public class GetUsersEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "users",
            async (
                IMessageBus bus,
                CancellationToken cancellationToken,
                [FromQuery(Name = "page")] int pageNumber = 1) =>
            {
                var pagination = new PaginationParameters(pageNumber);

                var query = new GetUsersQuery(pagination);

                var users = await bus
                    .InvokeAsync<PagedResponse<GetUsersResponse>>(query, cancellationToken);

                return Results.Ok(users);
            })
            .RequireAuthorization(UsersPolicies.AdminOnly)
            .WithTags(Tags.Users)
            .WithSummary("Get users")
            .Produces<PagedResponse<GetUsersResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}