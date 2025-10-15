using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Authentication.Logout;

public class LogoutEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("auth/logout", async (ISender mediator, HttpContext httpContext) =>
        {
            var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];

            var command = new LogoutCommand(refreshToken);

            var logoutResult = await mediator.Send(command);

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