using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Authentication.RefreshToken;

public class RefreshTokenEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/refresh-token",
            async (IMessageBus bus, HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];

                var command = new RefreshTokenCommand(refreshToken);

                var refreshTokenResult = await bus.InvokeAsync<ErrorOr<Success>>(command, cancellationToken);

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