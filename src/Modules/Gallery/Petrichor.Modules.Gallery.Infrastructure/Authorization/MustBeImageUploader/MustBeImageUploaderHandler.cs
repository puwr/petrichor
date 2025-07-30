using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;

namespace Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploader;

public class MustBeImageUploaderHandler(GalleryDbContext dbContext)
        : AuthorizationHandler<MustBeImageUploaderRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MustBeImageUploaderRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext) return;

        var imageIdFromRoute = httpContext.GetRouteValue("imageId")?.ToString();
        if (!Guid.TryParse(imageIdFromRoute, out Guid imageId)) return;

        var currentUserIdClaim = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
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