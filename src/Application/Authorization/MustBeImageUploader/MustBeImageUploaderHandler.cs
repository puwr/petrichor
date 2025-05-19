using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Authorization.MustBeImageUploader;

public class MustBeImageUploaderHandler(
    IImagesRepository imagesRepository) 
        : AuthorizationHandler<MustBeImageUploaderRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MustBeImageUploaderRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext) return;

        var imageIdFromRoute = httpContext.GetRouteValue("imageId").ToString();
        if (!Guid.TryParse(imageIdFromRoute, out Guid imageId)) return;

        var currentUserIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(currentUserIdClaim, out Guid currentUserId)) return;

        var uploaderId = await imagesRepository.GetUploaderIdAsync(imageId);

        if (uploaderId == currentUserId)
        {
            context.Succeed(requirement);
        }
    }
}