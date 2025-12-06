using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Authentication.Logout;

public class LogoutEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("auth/logout",
        async (IMessageBus bus, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];

            var command = new LogoutCommand(refreshToken);

            var logoutResult = await bus.InvokeAsync<ErrorOr<Success>>(command, cancellationToken);

            return logoutResult.Match(
                _ => Results.NoContent(),
                Problem
            );
        })
        .WithTags(Tags.Authentication)
        .WithSummary("Logout")
        .Produces(StatusCodes.Status204NoContent);
    }
}