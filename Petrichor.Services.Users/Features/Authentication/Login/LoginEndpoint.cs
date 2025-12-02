using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public class LoginEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/login",
            async (LoginRequest request, IMessageBus bus) =>
            {
                var query = new LoginCommand(request.Email, request.Password);

                var loginResult = await bus.InvokeAsync<ErrorOr<Success>>(query);

                return loginResult.Match(
                    _ => Results.NoContent(),
                    Problem
                );
            })
            .WithTags(Tags.Authentication)
            .WithSummary("Login")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
}