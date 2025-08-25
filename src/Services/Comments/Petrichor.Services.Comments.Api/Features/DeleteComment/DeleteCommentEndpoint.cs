using MediatR;
using Petrichor.Services.Comments.Api.Common.Authorization;

namespace Petrichor.Services.Comments.Api.Features.DeleteComment;

public class DeleteCommentEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapDelete(
            "api/comments/{commentId:guid}",
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
        .WithTags(Tags.Comments);
    }
}