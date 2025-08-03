using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Domain.Users;

namespace Petrichor.Modules.Users.Application.Common.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; }
    DbSet<IdentityRole<Guid>> Roles { get; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; }
}
