using Microsoft.AspNetCore.Authorization;
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
using Petrichor.Shared.Infrastructure.Outbox;

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

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GalleryDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "gallery"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IGalleryDbContext>(provider =>
            provider.GetRequiredService<GalleryDbContext>());

        services.AddHostedService<OutboxBackgroudService<GalleryDbContext>>();

        return services;
    }

    private static IServiceCollection AddAuthorizarion(this IServiceCollection services)
    {
        services.AddSingleton<IPostConfigureOptions<AuthorizationOptions>, GalleryAuthorizationConfiguration>();
        services.AddScoped<IAuthorizationHandler, MustBeImageUploaderHandler>();

        return services;
    }
}