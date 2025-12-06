using MemoryPack;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

[MemoryPackable]
public partial record GetUsersResponse(
    Guid Id,
    string Email,
    string UserName,
    List<string> Roles,
    bool IsDeleted);