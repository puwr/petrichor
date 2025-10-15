using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.Outbox;

namespace Petrichor.Services.Users.Common.Persistence;

public class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options),
        IInboxDbContext, IOutboxDbContext
{
    private static readonly Guid AdminRoleGuid = new("b31c98af-5964-4773-ab6c-cdc026b888ef");

    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<InboxMessageConsumer> InboxMessageConsumers { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }

    public Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());

        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

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