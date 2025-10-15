using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Services.Gallery.Common.Authorization.MustBeImageUploaderOrAdmin;
using Petrichor.Services.Gallery.Common.Extensions;
using Petrichor.Services.Gallery.Common.Inbox;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Services;
using Petrichor.Services.Gallery.Common.Storage;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Behaviors;
using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Events;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.IntegrationEvents;
using Petrichor.Shared.Outbox;
using Petrichor.Shared.Services.Storage;
using Petrichor.Shared.Services.Storage.Minio;
using Petrichor.Shared.Settings;

namespace Petrichor.Services.Gallery;

public static class DependencyInjection
{
    public static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        builder.Services.AddEndpoints();
    }

    public static void AddInfrastructure(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(builder.Configuration);

        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<GalleryDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("database"),
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "gallery"))
            .UseSnakeCaseNamingConvention());

        builder.AddMassTransitRabbitMq("rmq",
            options => options.DisableTelemetry = true,
            configure =>
            {
                configure.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
            });

        builder.Services.AddScoped<EventPublisher<GalleryDbContext>>();

        builder.Services.AddDomainEvents();
        builder.Services.AddIntegrationEvents();

        builder.Services.AddHostedService<InboxBackgroundService<GalleryDbContext>>();
        builder.Services.AddHostedService<OutboxBackgroudService<GalleryDbContext>>();


        builder.Services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        builder.Services.AddScoped<IImageMetadataProvider, ImageMetadataProvider>();

        builder.AddMinioClient("minio");
        builder.Services.AddScoped<IFileStorage, MinioFileStorage>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHostedService<MinioCreateBucketsTask>();
        }
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Key, jwtSettings);

        services
            .AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["ACCESS_TOKEN"];
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, MustBeImageUploaderOrAdminRequirementHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(GalleryPolicies.ImageUploaderOrAdmin, policy =>
                policy.AddRequirements(new MustBeImageUploaderOrAdminRequirement()));

        return services;
    }


    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddScoped<DomainEventDispatcher<GalleryDbContext>>();

        Type[] handlerTypes = typeof(DependencyInjection).Assembly
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

        Type[] handlerTypes = typeof(DependencyInjection).Assembly
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