using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Comments.Features.GetComments;

public record GetCommentsQuery(Guid ResourceId, CursorPaginationParameters PaginationParameters);
