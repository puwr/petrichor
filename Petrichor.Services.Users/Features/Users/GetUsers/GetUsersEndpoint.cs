using MediatR;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

public class GetUsersEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "users",
            async (ISender mediator, [FromQuery(Name = "page")] int pageNumber = 1) =>
            {
                var pagination = new PaginationParameters(pageNumber);

                var query = new GetUsersQuery(pagination);

                var listUsersResult = await mediator.Send(query);

                return listUsersResult.Match(
                    Results.Ok,
                    Problem
                );
            })
            .RequireAuthorization(UsersPolicies.AdminOnly)
            .WithTags(Tags.Users)
            .WithSummary("Get users")
            .Produces<PagedResponse<GetUsersResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}