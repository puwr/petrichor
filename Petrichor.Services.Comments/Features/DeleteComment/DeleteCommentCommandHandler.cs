using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.Features.DeleteComment;

public static class DeleteCommentCommandHandler
{
    public static async Task<ErrorOr<Deleted>> Handle(
        DeleteCommentCommand command,
        CommentsDbContext dbContext,
        IFusionCache cache,
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