namespace Petrichor.Shared.Pagination;

public record CursorPaginationParameters(string? Cursor = null, int Limit = 3);