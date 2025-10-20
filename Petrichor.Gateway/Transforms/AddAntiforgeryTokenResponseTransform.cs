using Microsoft.AspNetCore.Antiforgery;
using Yarp.ReverseProxy.Transforms;

namespace Petrichor.Gateway.Transforms;

public class AddAntiforgeryTokenResponseTransform(IAntiforgery antiforgery) : ResponseTransform
{
    public override ValueTask ApplyAsync(ResponseTransformContext context)
    {
        if (!context.HttpContext.Request.RouteValues.ContainsKey("catch-all"))
        {
            return ValueTask.CompletedTask;
        }

        var tokenSet = antiforgery.GetAndStoreTokens(context.HttpContext);

        context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return ValueTask.CompletedTask;
    }
}