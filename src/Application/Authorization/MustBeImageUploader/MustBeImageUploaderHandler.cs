using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Application.Authorization.MustBeImageUploader;

public class MustBeImageUploaderHandler(IPetrichorDbContext dbContext) 
        : AuthorizationHandler<MustBeImageUploaderRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MustBeImageUploaderRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext) return;

        var imageIdFromRoute = httpContext.GetRouteValue("imageId")?.ToString();
        if (!Guid.TryParse(imageIdFromRoute, out Guid imageId)) return;

        var currentUserIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(currentUserIdClaim, out Guid currentUserId)) return;


        var uploaderId = await dbContext.Images
            .AsNoTracking()
            .Where(i => i.Id == imageId)
            .Select(i => i.UploaderId)
            .FirstOrDefaultAsync();

        if (uploaderId == currentUserId)
        {
            context.Succeed(requirement);
        }
    }
}