using MemoryPack;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

[MemoryPackable]
public partial record GetUsersResponse(
    Guid Id,
    string Email,
    string UserName,
    DateTime RegisteredAt,
    List<string> Roles,
    bool IsDeleted);