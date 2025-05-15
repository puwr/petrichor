using System.Reflection;
using Application.Common.Interfaces;
using Domain.Images;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;

public class PetrichorDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public async Task CommitChangesAsync()
    {
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}