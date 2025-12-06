using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Comments.Features.CreateComment;

public class CreateCommentEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "comments",
            async (
                CreateCommentRequest request,
                IMessageBus bus,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var authorId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

                var command = new CreateCommentCommand(
                    authorId,
                    request.ResourceId,
                    request.Message);

                var createdCommentId = await bus.InvokeAsync<Guid>(command, cancellationToken);

                return Results.Created($"/comments?resourceId={request.ResourceId}", createdCommentId);
            })
            .RequireAuthorization()
            .WithTags(Tags.Comments)
            .WithSummary("Create comment")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}
