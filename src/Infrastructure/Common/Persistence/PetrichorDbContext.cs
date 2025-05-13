using System.Reflection;
using Domain.Images;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;

public class PetrichorDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}