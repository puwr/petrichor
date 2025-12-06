using System.Security.Claims;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public class GetCurrentUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "account/me",
            async (IMessageBus bus, ClaimsPrincipal user) =>
            {
                var query = new GetCurrentUserQuery(user);

                var response = await bus
                    .InvokeAsync<GetCurrentUserResponse>(query);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags(Tags.Account)
            .WithSummary("Get current user")
            .Produces<GetCurrentUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}