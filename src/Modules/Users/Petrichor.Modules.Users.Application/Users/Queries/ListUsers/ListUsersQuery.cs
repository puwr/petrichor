using ErrorOr;
using MediatR;
using Petrichor.Modules.Users.Contracts.Users;
using Petrichor.Shared.Pagination;

namespace Petrichor.Modules.Users.Application.Users.Queries.ListUsers;

public record ListUsersQuery(PaginationParameters Pagination) : IRequest<ErrorOr<PagedResponse<ListUsersResponse>>>;
