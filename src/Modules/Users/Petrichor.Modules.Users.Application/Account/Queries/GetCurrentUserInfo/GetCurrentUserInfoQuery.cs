using ErrorOr;
using MediatR;
using Petrichor.Modules.Users.Contracts.Account;

namespace Petrichor.Modules.Users.Application.Account.Queries.GetCurrentUserInfo;

public record GetCurrentUserInfoQuery: IRequest<ErrorOr<GetCurrentUserInfoResponse>>;