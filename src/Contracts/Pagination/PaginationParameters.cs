namespace Contracts.Pagination;

public record PaginationParameters(int PageNumber = 1, int PageSize = 10)
{
    public int Skip => (PageNumber - 1) * PageSize;
}
