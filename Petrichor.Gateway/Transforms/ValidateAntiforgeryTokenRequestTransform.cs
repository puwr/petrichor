using Microsoft.AspNetCore.Antiforgery;
using Yarp.ReverseProxy.Transforms;

namespace Petrichor.Gateway.Transforms;

public class ValidateAntiforgeryTokenRequestTransform(IAntiforgery antiforgery) : RequestTransform
{
    public override async ValueTask ApplyAsync(RequestTransformContext context)
    {
        var path = context.HttpContext.Request.Path.Value;

        if (context.HttpContext.Request.Method == HttpMethod.Get.Method ||
            (path is not null && path.Contains("/auth/")))
            return;

        try
        {
            var token = context.HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "XSRF-TOKEN");
            context.HttpContext.Request.Headers.Append("XSRF-TOKEN", token.Value!);
            await antiforgery.ValidateRequestAsync(context.HttpContext);
        }
        catch
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}