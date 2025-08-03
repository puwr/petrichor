using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Users.Infrastructure.Persistence;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), IUsersDbContext, IOutboxDbContext
{
    private static readonly Guid AdminRoleGuid = new("b31c98af-5964-4773-ab6c-cdc026b888ef");

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        modelBuilder.Entity<IdentityRole<Guid>>()
            .HasData(new IdentityRole<Guid>
            {
                Id = AdminRoleGuid,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

        base.OnModelCreating(modelBuilder);
    }
}