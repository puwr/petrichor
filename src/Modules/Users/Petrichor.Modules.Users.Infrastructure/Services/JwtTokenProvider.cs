using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Modules.Users.Application.Common.Interfaces.Services;
using Petrichor.Modules.Users.Application.Common.Interfaces.Services.Authentication;
using Petrichor.Modules.Users.Contracts.Authentication;
using Petrichor.Modules.Users.Domain.Users;

namespace Petrichor.Modules.Users.Infrastructure.Services;

public class JwtTokenProvider(
    IOptions<JwtSettings> jwtSettings) : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public TokenResult GenerateAccessToken(User user, IList<string> roles)
    {
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!)
        }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var accessTokenExpirationDateInUtc =
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationTimeInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims,
            expires: accessTokenExpirationDateInUtc,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var tokenResult = new TokenResult(
            Token: accessToken,
            ExpiresAt: accessTokenExpirationDateInUtc
        );

        return tokenResult;
    }

    public TokenResult GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenTokenExpirationTimeInDays);

        var tokenResult = new TokenResult(
            Token: refreshToken,
            ExpiresAt: refreshTokenExpirationDateInUtc
        );

        return tokenResult;
    }
}