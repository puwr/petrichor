using System.Text.RegularExpressions;
using ErrorOr;
using MediatR;
using Petrichor.Services.Comments.Common.Domain;
using Petrichor.Services.Comments.Common.Persistence;

namespace Petrichor.Services.Comments.Features.CreateComment;

public class CreateCommentCommandHandler(CommentsDbContext dbContext)
    : IRequestHandler<CreateCommentCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        CreateCommentCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedMessage = StripHtmlTags(command.Message.Trim());

        var comment = Comment.Create(
            authorId: command.AuthorId,
            resourceId: command.ResourceId,
            message: normalizedMessage);

        dbContext.Comments.Add(comment);

        await dbContext.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }

    private static string StripHtmlTags(string input)
    {
        return string.IsNullOrEmpty(input)
                    ? input
                    : Regex.Replace(input, @"<[^>]+>", string.Empty);
    }
}