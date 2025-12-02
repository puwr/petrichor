using ErrorOr;
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
            async (Guid userId, IMessageBus bus, [FromQuery] bool deleteUploadedImages = false) =>
        {
            var command = new DeleteUserCommand(userId, deleteUploadedImages);

            var deleteUserResult = await bus.InvokeAsync<ErrorOr<Deleted>>(command);

            return deleteUserResult.Match(
                _ => Results.NoContent(),
                Problem
            );
        })
        .RequireAuthorization(UsersPolicies.AdminOnly)
        .WithTags(Tags.Users)
        .WithSummary("Delete user")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}