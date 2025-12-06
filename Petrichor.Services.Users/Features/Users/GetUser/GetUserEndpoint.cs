using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Users.GetUser;

public class GetUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "users/{userId:guid}",
            async (Guid userId, IMessageBus bus, CancellationToken cancellationToken) =>
            {
                var query = new GetUserQuery(userId);

                var getUserResult = await bus
                    .InvokeAsync<ErrorOr<GetUserResponse>>(query, cancellationToken);

                return getUserResult.Match(
                    Results.Ok,
                    Problem
                );
            })
            .WithTags(Tags.Users)
            .WithSummary("Get user")
            .Produces<GetUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}