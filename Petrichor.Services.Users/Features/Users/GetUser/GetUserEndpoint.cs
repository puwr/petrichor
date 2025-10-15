using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Users.GetUser;

public class GetUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("users/{userId:guid}", async (Guid userId, ISender mediator) =>
        {
            var query = new GetUserQuery(userId);

            var getUserResult = await mediator.Send(query);

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