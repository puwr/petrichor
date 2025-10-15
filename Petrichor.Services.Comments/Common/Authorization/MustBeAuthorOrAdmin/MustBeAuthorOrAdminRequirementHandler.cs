using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;

namespace Petrichor.Services.Comments.Common.Authorization.MustBeAuthorOrAdmin;

public class MustBeAuthorOrAdminRequirementHandler(CommentsDbContext dbContext)
        : AuthorizationHandler<MustBeAuthorOrAdminRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MustBeAuthorOrAdminRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext) return;

        if (httpContext.User.IsInRole("Admin"))
            context.Succeed(requirement);

        var commentIdFromRoute = httpContext.GetRouteValue("commentId")?.ToString();
        if (!Guid.TryParse(commentIdFromRoute, out Guid commentId)) return;

        var currentUserIdClaim = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(currentUserIdClaim, out Guid currentUserId)) return;

        var uploaderId = await dbContext.Comments
            .AsNoTracking()
            .Where(i => i.Id == commentId)
            .Select(i => i.AuthorId)
            .FirstOrDefaultAsync();

        if (uploaderId == currentUserId)
        {
            context.Succeed(requirement);
        }
    }
}