using Petrichor.Services.Comments.Common.Authorization;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Comments.Features.DeleteComment;

public class DeleteCommentEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "comments/{commentId:guid}",
            async (Guid commentId, IMessageBus bus, CancellationToken cancellationToken) =>
            {
                var command = new DeleteCommentCommand(commentId);

                await bus.InvokeAsync(command, cancellationToken);

                return Results.NoContent();
            }
        )
        .RequireAuthorization(CommentsPolicies.AuthorOrAdmin)
        .WithTags(Tags.Comments)
        .WithSummary("Delete comment")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);
    }
}