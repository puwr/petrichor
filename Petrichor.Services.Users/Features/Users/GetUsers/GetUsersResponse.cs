namespace Petrichor.Services.Users.Features.Users.GetUsers;

public record GetUsersResponse(
    Guid Id,
    string Email,
    string UserName,
    List<string> Roles,
    bool IsDeleted);