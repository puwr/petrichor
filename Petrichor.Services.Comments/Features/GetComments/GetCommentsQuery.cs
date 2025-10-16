using ErrorOr;
using MediatR;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Comments.Features.GetComments;

public record GetCommentsQuery(Guid ResourceId, CursorPaginationParameters PaginationParameters)
    : IRequest<ErrorOr<CursorPagedResponse<GetCommentsResponse>>>;
