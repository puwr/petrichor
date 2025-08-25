using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Api.Common.Persistence;

namespace Petrichor.Services.Comments.Api.Common.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using CommentsDbContext context = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();

        context.Database.Migrate();
    }
}
