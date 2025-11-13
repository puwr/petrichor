using System.Security.Claims;
using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public record GetCurrentUserQuery(ClaimsPrincipal User)
    : IRequest<ErrorOr<GetCurrentUserResponse>>;