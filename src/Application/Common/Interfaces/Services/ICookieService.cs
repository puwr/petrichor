using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface ICookieService
{
    void WriteCookie(
        string name,
        string value,
        DateTime expiresAt,
        bool isEssential = true,
        bool httpOnly = true,
        bool secure = true,
        SameSiteMode sameSite = SameSiteMode.Strict);

    void DeleteCookie(
        string name,
        bool httpOnly = true,
        bool secure = true,
        SameSiteMode sameSite = SameSiteMode.Strict);
}