using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Shared.Behaviors;

namespace Petrichor.Modules.Shared.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleAssemblies);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(moduleAssemblies);

        return services;
    }
}
