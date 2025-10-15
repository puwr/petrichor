using System.Security.Claims;
using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUserInfo;

public record GetCurrentUserInfoQuery(ClaimsPrincipal User)
    : IRequest<ErrorOr<GetCurrentUserInfoResponse>>;