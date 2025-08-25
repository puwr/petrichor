using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Api.Common.Persistence;

namespace Petrichor.Services.Comments.Api.Features.DeleteComment;

public class DeleteCommentCommandHandler(CommentsDbContext dbContext)
    : IRequestHandler<DeleteCommentCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        await dbContext.Comments
            .Where(c => c.Id == command.CommentId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Deleted;
    }
}