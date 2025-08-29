namespace Petrichor.Modules.Users.Contracts.Account;

public record GetCurrentUserInfoResponse(string Id, string Email, string UserName, List<string> Roles);