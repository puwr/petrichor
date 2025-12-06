using System.Text.RegularExpressions;
using Petrichor.Services.Comments.Common.Domain;
using Petrichor.Services.Comments.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.Features.CreateComment;

public static class CreateCommentCommandHandler
{
    public static async Task<Guid> Handle(
        CreateCommentCommand command,
        CommentsDbContext dbContext,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var normalizedMessage = StripHtmlTags(command.Message.Trim());

        var comment = Comment.Create(
            authorId: command.AuthorId,
            resourceId: command.ResourceId,
            message: normalizedMessage);

        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(
            $"comments:{comment.ResourceId}",
            token: cancellationToken);

        return comment.Id;
    }

    private static string StripHtmlTags(string input)
    {
        return string.IsNullOrEmpty(input)
                    ? input
                    : Regex.Replace(input, @"<[^>]+>", string.Empty);
    }
}