using MediatR;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public class DeleteUserEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "users/{userId:guid}",
            async (Guid userId, [FromBody] DeleteUserRequest request, ISender mediator) =>
        {
            var command = new DeleteUserCommand(userId, request.DeleteUploadedImages);

            var deleteUserResult = await mediator.Send(command);

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