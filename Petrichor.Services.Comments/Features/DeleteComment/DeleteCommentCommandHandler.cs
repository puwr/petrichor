using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.Features.DeleteComment;

public class DeleteCommentCommandHandler(CommentsDbContext dbContext, IFusionCache cache)
    : IRequestHandler<DeleteCommentCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        var comment = await dbContext.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == command.CommentId,
                cancellationToken);

        if (comment is null)
        {
            return Result.Deleted;
        }

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(
            $"comments:{comment.ResourceId}",
            token: cancellationToken);

        return Result.Deleted;
    }
}