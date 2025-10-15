using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUserInfo;

public class GetCurrentUserInfoEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("account/me", async (ISender mediator, HttpContext httpContext) =>
        {
            var query = new GetCurrentUserInfoQuery(httpContext.User);

            var getCurrentUserInfoResult = await mediator.Send(query);

            return getCurrentUserInfoResult.Match(
                Results.Ok,
                Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Account)
        .WithSummary("Get current user")
        .Produces<GetCurrentUserInfoResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}