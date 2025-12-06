using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Shared.Extensions;
using ZiggyCreatures.Caching.Fusion;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Comments.Features.GetComments;

public static class GetCommentsQueryHandler
{
    public static async Task<CursorPagedResponse<GetCommentsResponse>> Handle(
        GetCommentsQuery query,
        IDbContextFactory<CommentsDbContext> dbContextFactory,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var response = await cache.GetOrSetAsync(
            $"comments:{query.ResourceId}:{query.Pagination.Cursor ?? "null"}",
            async _ =>
            {
                var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

                var commemtsQuery = dbContext.Comments.AsNoTracking().AsQueryable();

                var lastId = Cursor.Decode(query.Pagination.Cursor);
                commemtsQuery = commemtsQuery.WhereIf(
                    condition: lastId.HasValue,
                    predicate: comment => comment.Id <= lastId!.Value
                );

                var comments = await commemtsQuery
                    .Where(c => c.ResourceId == query.ResourceId)
                    .OrderByDescending(c => c.Id)
                    .Take(query.Pagination.Limit + 1)
                    .GroupJoin(dbContext.UserSnapshots,
                        comment => comment.AuthorId,
                        userSnapshot => userSnapshot.UserId,
                        (comment, userSnapshots) => new
                        { Comment = comment, UserSnapshot = userSnapshots.FirstOrDefault() })
                    .Select(x => GetCommentsResponse.From(x.Comment, x.UserSnapshot))
                    .ToListAsync(cancellationToken);

                var hasMore = comments.Count > query.Pagination.Limit;

                string? nextCursor = null;

                if (hasMore)
                {
                    nextCursor = Cursor.Encode(comments[^1].Id);

                    comments.RemoveAt(comments.Count - 1);
                }

                return new CursorPagedResponse<GetCommentsResponse>(
                    Items: comments,
                    NextCursor: nextCursor,
                    HasMore: hasMore);
            },
            tags: [$"comments:{query.ResourceId}"],
            token: cancellationToken);

        return response;
    }
}