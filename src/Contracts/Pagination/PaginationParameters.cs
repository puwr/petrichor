namespace Contracts.Pagination;

public record PaginationParameters(int PageNumber = 1)
{
    public int Skip => (PageNumber - 1) * PageSize;
    public int PageSize { get; } = 10;
}
