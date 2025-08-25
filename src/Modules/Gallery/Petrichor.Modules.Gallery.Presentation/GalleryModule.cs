using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;
using Petrichor.Modules.Gallery.Infrastructure.Authorization;
using Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploader;
using Petrichor.Modules.Gallery.Infrastructure.Inbox;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using Petrichor.Modules.Gallery.Infrastructure.Services;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.IntegrationEvents;
using Petrichor.Shared.Outbox;
using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Events;
using Petrichor.Modules.Users.IntegrationMessages;
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

        services.AddScoped<EventPublisher<IGalleryDbContext>>();
        services.AddDomainEvents();
        services.AddIntegrationEvents();

        services.AddHostedService<InboxBackgroundService<GalleryDbContext>>();
        services.AddHostedService<OutboxBackgroudService<GalleryDbContext>>();

        services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        services.AddScoped<IImageMetadataProvider, ImageMetadataProvider>();

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GalleryDbContext>((sp, options) =>
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

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddScoped<DomainEventDispatcher<GalleryDbContext>>();

        Type[] handlerTypes = Application.AssemblyMarker.Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
            .ToArray();

        foreach (Type handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            services.TryAddScoped(interfaceType, handlerType);
        }

        return services;
    }

    private static IServiceCollection AddIntegrationEvents(this IServiceCollection services)
    {
        services.AddScoped<IntegrationEventDispatcher<GalleryDbContext>>();

        Type[] handlerTypes = Application.AssemblyMarker.Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
            .ToArray();

        foreach (Type handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

            services.TryAddScoped(interfaceType, handlerType);
        }

        return services;
    }
}