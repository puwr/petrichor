using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Api.Common.Persistence;
using Petrichor.Shared.Extensions;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Comments.Api.Features.GetComments;

public class GetCommentsQueryHandler(CommentsDbContext dbContext)
    : IRequestHandler<GetCommentsQuery, ErrorOr<CursorPagedResponse<CommentResponse>>>
{
    public async Task<ErrorOr<CursorPagedResponse<CommentResponse>>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Comments.AsNoTracking().AsQueryable();

        var lastId = Cursor.Decode(request.PaginationParameters.Cursor);

        query = query.WhereIf(
            condition: lastId.HasValue,
            predicate: comment => comment.Id <= lastId!.Value
        );

        var comments = await query
            .Where(c => c.ResourceId == request.ResourceId)
            .OrderByDescending(c => c.Id)
            .Take(request.PaginationParameters.Limit + 1)
            .GroupJoin(dbContext.UserSnapshots,
                comment => comment.AuthorId,
                userSnapshot => userSnapshot.UserId,
                (comment, userSnapshots) => new
                    { Comment = comment, UserSnapshot = userSnapshots.FirstOrDefault() })
            .Select(x => CommentResponse.From(x.Comment, x.UserSnapshot))
            .ToListAsync(cancellationToken);

        var hasMore = comments.Count > request.PaginationParameters.Limit;

        string? nextCursor = null;

        if (hasMore)
        {
            nextCursor = Cursor.Encode(comments[^1].Id);

            comments.RemoveAt(comments.Count - 1);
        }

        return new CursorPagedResponse<CommentResponse>(
            Items: comments,
            NextCursor: nextCursor,
            HasMore: hasMore);;
    }
}