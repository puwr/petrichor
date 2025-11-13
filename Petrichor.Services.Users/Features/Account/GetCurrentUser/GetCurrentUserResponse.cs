namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public record GetCurrentUserResponse(string Id, string Email, string UserName, List<string> Roles);