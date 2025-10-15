using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Persistence;

namespace Petrichor.Services.Gallery.Common.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using GalleryDbContext context = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();

        context.Database.Migrate();
    }
}