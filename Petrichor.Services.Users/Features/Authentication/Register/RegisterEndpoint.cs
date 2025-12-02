using ErrorOr;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Authentication.Register;

public class RegisterEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "auth/register",
            async (RegisterRequest request, IMessageBus bus) =>
            {
                var command = new RegisterCommand(
                    request.Email.ToLowerInvariant(),
                    request.UserName.ToLowerInvariant(),
                    request.Password);

                var registerResult = await bus.InvokeAsync<ErrorOr<Guid>>(command);

                return registerResult.Match(
                    userId => Results.Created($"/users/{userId}", userId),
                    Problem
                );
            })
            .WithTags(Tags.Authentication)
            .WithSummary("Register")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }
}