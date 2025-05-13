using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<PetrichorDbContext>(options =>
            options.UseSqlite("Data Source = Petrichor.db"));

        return services;
    }
}