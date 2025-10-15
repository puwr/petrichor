using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Authentication.RefreshToken;

public class RefreshTokenEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/refresh-token",
            async (ISender mediator, HttpContext httpContext) =>
            {
                var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];

                var command = new RefreshTokenCommand(refreshToken);

                var refreshTokenResult = await mediator.Send(command);

                return refreshTokenResult.Match(
                    _ => Results.NoContent(),
                    Problem
                );
            })
            .WithTags(Tags.Authentication)
            .WithSummary("Refresh token")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}