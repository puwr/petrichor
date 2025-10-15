using MediatR;
using Petrichor.Services.Comments.Common.Authorization;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Comments.Features.DeleteComment;

public class DeleteCommentEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "comments/{commentId:guid}",
            async (Guid commentId, ISender mediator) =>
            {
                var command = new DeleteCommentCommand(commentId);

                var deleteCommentResult = await mediator.Send(command);

                return deleteCommentResult.Match(
                    _ => Results.NoContent(),
                    Problem);
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