using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Domain;
using Petrichor.Services.Comments.Common.Persistence.Configurations;
using Wolverine.EntityFrameworkCore;

namespace Petrichor.Services.Comments.Common.Persistence;

public class CommentsDbContext(DbContextOptions<CommentsDbContext> options)
    : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserSnapshot> UserSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("comments");

        modelBuilder.MapWolverineEnvelopeStorage();

        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new UserSnapshotConfiguration());

        base.OnModelCreating(modelBuilder);
    }

}