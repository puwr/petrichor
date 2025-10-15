using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public class LoginEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/login",
            async (LoginRequest request, ISender mediator) =>
            {
                var query = new LoginCommand(request.Email, request.Password);

                var loginResult = await mediator.Send(query);

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