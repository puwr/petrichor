using Contracts.Account;
using ErrorOr;
using MediatR;

namespace Application.Account.Queries.GetCurrentUserInfo;

public record GetCurrentUserInfoQuery: IRequest<ErrorOr<GetCurrentUserInfoResponse>>;