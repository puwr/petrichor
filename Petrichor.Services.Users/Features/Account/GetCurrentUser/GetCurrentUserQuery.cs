using System.Security.Claims;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public record GetCurrentUserQuery(ClaimsPrincipal User);