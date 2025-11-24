using Microsoft.AspNetCore.Identity;

namespace Petrichor.Services.Users.Common.Persistence;

public class RolesSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        const string adminRoleName = "Admin";

        var role = await roleManager.FindByNameAsync(adminRoleName);

        if (role is null)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>
            {
              Id = Guid.Parse("b31c98af-5964-4773-ab6c-cdc026b888ef"),
              Name = adminRoleName,
              NormalizedName = adminRoleName.ToUpperInvariant()
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}