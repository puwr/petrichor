using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Authentication.Register;

public class RegisterEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/register",
            async (RegisterRequest request, ISender mediator) =>
            {
                var command = new RegisterCommand(request.Email, request.UserName, request.Password);

                var registerResult = await mediator.Send(command);

                return registerResult.Match(
                    _ => Results.NoContent(),
                    Problem
                );
            })
            .WithTags(Tags.Authentication)
            .WithSummary("Register")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }
}