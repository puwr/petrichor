using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public class DeleteUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "users/{userId:guid}",
            async (
                Guid userId,
                IMessageBus bus,
                CancellationToken cancellationToken,
                [FromQuery] bool deleteUploadedImages = false) =>
        {
            var command = new DeleteUserCommand(userId, deleteUploadedImages);

            await bus.InvokeAsync(command, cancellationToken);

            return Results.NoContent();
        })
        .RequireAuthorization(UsersPolicies.AdminOnly)
        .WithTags(Tags.Users)
        .WithSummary("Delete user")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}