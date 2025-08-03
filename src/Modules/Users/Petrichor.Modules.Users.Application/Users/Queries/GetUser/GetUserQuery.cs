using ErrorOr;
using MediatR;
using Petrichor.Modules.Users.Contracts.Account;

namespace Petrichor.Modules.Users.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<GetUserResponse>>;