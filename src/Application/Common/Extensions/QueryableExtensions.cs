using System.Linq.Expressions;
using Contracts.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
        => condition ? query.Where(predicate) : query;

    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var count = await query
            .CountAsync(cancellationToken: cancellationToken);

        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<T>(
            items,
            count,
            pagination.PageNumber,
            pagination.PageSize);
    }
}
