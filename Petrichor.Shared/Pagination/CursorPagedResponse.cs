namespace Petrichor.Shared.Pagination;

public record CursorPagedResponse<T>(IReadOnlyList<T> Items, string? NextCursor, bool HasMore);