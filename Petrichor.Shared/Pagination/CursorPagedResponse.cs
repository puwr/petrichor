using MemoryPack;

namespace Petrichor.Shared.Pagination;

[MemoryPackable]
public partial record CursorPagedResponse<T>(IReadOnlyList<T> Items, string? NextCursor, bool HasMore);