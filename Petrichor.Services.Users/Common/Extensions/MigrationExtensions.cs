using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Persistence;

namespace Petrichor.Services.Users.Common.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using UsersDbContext context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        context.Database.Migrate();
    }
}
