using MemoryPack;

namespace Petrichor.Shared.Pagination;

[MemoryPackable]
public partial record PagedResponse<T>(IReadOnlyList<T> Items, int Count, int PageNumber, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(Count / (double)PageSize);
}
