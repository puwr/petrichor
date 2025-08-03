namespace Petrichor.Modules.Users.Contracts.Users;

public record ListUsersResponse(Guid Id, string Email, string UserName, List<string> Roles);