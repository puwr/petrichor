using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Contracts.Account;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Account.Queries.GetCurrentUserInfo;

public class GetCurrentUserInfoQueryHandler(
    IHttpContextAccessor httpContextAccessor
    ) : IRequestHandler<GetCurrentUserInfoQuery,
        ErrorOr<GetCurrentUserInfoResponse>>
{
    public Task<ErrorOr<GetCurrentUserInfoResponse>> Handle(
        GetCurrentUserInfoQuery request,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var id = user?
            .FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(id))
        {
            return Task.FromResult(Error
                .Failure("User id claim is missing.")
                .ToErrorOr<GetCurrentUserInfoResponse>());
        }

        var email = user?
            .FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrEmpty(email))
        {
            return Task.FromResult(Error
            .Failure("User email claim is missing.")
            .ToErrorOr<GetCurrentUserInfoResponse>());
        }

        var userName = user?
            .FindFirstValue(JwtRegisteredClaimNames.UniqueName);

        if (string.IsNullOrEmpty(userName))
        {
            return Task.FromResult(Error
            .Failure("User name claim is missing.")
            .ToErrorOr<GetCurrentUserInfoResponse>());
        }

        return Task.FromResult(new GetCurrentUserInfoResponse(
            id,
            email,
            userName
        ).ToErrorOr());
    }
}
