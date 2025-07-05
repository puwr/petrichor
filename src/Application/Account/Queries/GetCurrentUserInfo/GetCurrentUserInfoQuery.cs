using Contracts.Account;
using ErrorOr;
using Mediator;

namespace Application.Account.Queries.GetCurrentUserInfo;

public record GetCurrentUserInfoQuery: IRequest<ErrorOr<GetCurrentUserInfoResponse>>;