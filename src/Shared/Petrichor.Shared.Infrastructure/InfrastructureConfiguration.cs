using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Shared.Infrastructure.Outbox;
using Petrichor.Shared.Infrastructure.Services.Storage;

namespace Petrichor.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder builder)
    {
        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Petrichor",
                    StorageFolders.Uploads)),
            RequestPath = "/uploads"
        });

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Petrichor",
                    StorageFolders.Thumbnails)),
            RequestPath = "/thumbs"
        });

        return builder;
    }
}
