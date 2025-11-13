using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public class GetCurrentUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("account/me", async (ISender mediator, HttpContext httpContext) =>
        {
            var query = new GetCurrentUserQuery(httpContext.User);

            var getCurrentUserInfoResult = await mediator.Send(query);

            return getCurrentUserInfoResult.Match(
                Results.Ok,
                Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Account)
        .WithSummary("Get current user")
        .Produces<GetCurrentUserResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}