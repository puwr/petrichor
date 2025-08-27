using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Shared.Behaviors;

namespace Petrichor.Modules.Shared.Application;

public static class ApplicationConfiguration
{
    public static void AddApplication(this WebApplicationBuilder builder, Assembly[] moduleAssemblies)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleAssemblies);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssemblies(moduleAssemblies);
    }
}
