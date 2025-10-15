using ErrorOr;
using MediatR;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

public record GetUsersQuery(PaginationParameters Pagination)
    : IRequest<ErrorOr<PagedResponse<GetUsersResponse>>>;
