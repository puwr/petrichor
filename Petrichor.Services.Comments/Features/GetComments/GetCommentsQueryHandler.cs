using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Shared.Extensions;
using Petrichor.Shared.Pagination;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Comments.Features.GetComments;

public class GetCommentsQueryHandler(IServiceScopeFactory scopeFactory, IFusionCache cache)
    : IRequestHandler<GetCommentsQuery, ErrorOr<CursorPagedResponse<GetCommentsResponse>>>
{
    public async Task<ErrorOr<CursorPagedResponse<GetCommentsResponse>>> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await cache.GetOrSetAsync(
            $"comments:{request.ResourceId}:{request.PaginationParameters.Cursor ?? "null"}",
            async _ =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();

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
                    .Select(x => GetCommentsResponse.From(x.Comment, x.UserSnapshot))
                    .ToListAsync(cancellationToken);

                var hasMore = comments.Count > request.PaginationParameters.Limit;

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
            tags: [$"comments:{request.ResourceId}"],
            token: cancellationToken);

        return response;
    }
}