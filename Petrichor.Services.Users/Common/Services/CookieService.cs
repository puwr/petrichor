namespace Petrichor.Services.Users.Common.Services;

public class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
    public void WriteCookie(
        string name,
        string value,
        DateTime expiresAt,
        bool isEssential = true,
        bool httpOnly = true,
        bool secure = true,
        SameSiteMode sameSite = SameSiteMode.Strict)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(name, value,
            new CookieOptions
            {
                Expires = expiresAt,
                IsEssential = isEssential,
                HttpOnly = httpOnly,
                Secure = secure,
                SameSite = sameSite
            });
    }

    public void DeleteCookie(
        string name,
        bool httpOnly = true,
        bool secure = true,
        SameSiteMode sameSite = SameSiteMode.Strict)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(name,
        new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = secure,
            SameSite = sameSite
        });
    }
}
