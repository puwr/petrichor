using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class JwtTokenProvider(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor) : IJwtTokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public (string accessToken, DateTime accessTokenExpirationDateInUtc) GenerateAccessToken(User user)
    {
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.UserName!)
        };

        var accessTokenExpirationDateInUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationTimeInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims,
            expires: accessTokenExpirationDateInUtc,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, accessTokenExpirationDateInUtc);
    }

    public (string refreshToken, DateTime refreshTokenExpirationDateInUtc) GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(
            _jwtSettings.RefreshTokenTokenExpirationTimeInDays);

        return (refreshToken, refreshTokenExpirationDateInUtc);
    }

    public void WriteTokenAsHttpOnlyCookie(
        string cookieName, string token, DateTime expiresAt)
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Append(cookieName, token,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresAt,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
    }
}