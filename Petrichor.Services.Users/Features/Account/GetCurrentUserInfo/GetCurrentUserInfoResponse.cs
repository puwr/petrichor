namespace Petrichor.Services.Users.Features.Account.GetCurrentUserInfo;

public record GetCurrentUserInfoResponse(string Id, string Email, string UserName, List<string> Roles);