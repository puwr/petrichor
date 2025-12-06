using MemoryPack;

namespace Petrichor.Services.Users.Features.Users.GetUser;

[MemoryPackable]
public partial record GetUserResponse(Guid Id, string UserName);
