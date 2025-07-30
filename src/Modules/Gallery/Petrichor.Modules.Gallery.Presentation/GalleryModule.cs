using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using Petrichor.Modules.Gallery.Infrastructure.Authorization;
using Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploader;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using Petrichor.Modules.Gallery.Infrastructure.Services;
using Petrichor.Modules.Gallery.Presentation.Middleware;

namespace Petrichor.Modules.Gallery.Presentation;

public static class GalleryModule
{
    public static IServiceCollection AddGalleryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddAuthorizarion();

        services.AddControllers()
            .AddApplicationPart(typeof(GalleryModule).Assembly);

        services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        services.AddScoped<IImageMetadataProvider, ImageMetadataProvider>();

        return services;
    }

    public static IApplicationBuilder UseGalleryMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<EventualConsistencyMiddleware>();
        return builder;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GalleryDbContext>(options =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "gallery"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IGalleryDbContext>(provider =>
            provider.GetRequiredService<GalleryDbContext>());

        return services;
    }

    private static IServiceCollection AddAuthorizarion(this IServiceCollection services)
    {
        services.AddSingleton<IPostConfigureOptions<AuthorizationOptions>, GalleryAuthorizationConfiguration>();
        services.AddScoped<IAuthorizationHandler, MustBeImageUploaderHandler>();

        return services;
    }
}
