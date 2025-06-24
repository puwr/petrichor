namespace Contracts.Pagination;

public record PagedResponse<T>(IReadOnlyList<T> Items, int Count, int PageNumber, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(Count / (double)PageSize);
}
