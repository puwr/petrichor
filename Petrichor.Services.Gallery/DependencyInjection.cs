using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Services.Gallery.Common.Authorization;
using Petrichor.Services.Gallery.Common.Authorization.MustBeImageUploaderOrAdmin;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Common.Services;
using Petrichor.Services.Gallery.Common.Storage;
using Petrichor.Shared.Services.Storage;
using Petrichor.Shared.Services.Storage.Minio;
using Petrichor.Shared.Settings;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Petrichor.Services.Gallery;

public static class DependencyInjection
{
    public static void AddGallery(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        builder.Services.AddScoped<IImageMetadataProvider, ImageMetadataProvider>();

        builder.AddMinioClient("minio");
        builder.Services.AddScoped<IFileStorage, MinioFileStorage>();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHostedService<MinioCreateBucketsTask>();
        }
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.");

        services.AddDbContext<GalleryDbContext>(options =>
            options.UseNpgsql(
                databaseConnectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "gallery"))
            .UseSnakeCaseNamingConvention(),
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddPooledDbContextFactory<GalleryDbContext>(options =>
            options.UseNpgsql(
                databaseConnectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "gallery"))
            .UseSnakeCaseNamingConvention());

        return services;
    }

    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        var databaseConnectionString = builder.Configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.");

        var rmqConnectionString = builder.Configuration.GetConnectionString("rmq")
            ?? throw new InvalidOperationException("Connection string 'rmq' not found.");

        builder.UseWolverine(options =>
        {
            options.PersistMessagesWithPostgresql(databaseConnectionString, "gallery");
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.UseDurableLocalQueues();
            options.Policies.UseDurableInboxOnAllListeners();
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();
            options.Policies.AutoApplyTransactions();

            options.UseRabbitMq(rmqConnectionString)
                .AutoProvision()
                .BindExchange("users-exchange").ToQueue("users-gallery");

            options.ListenToRabbitQueue("users-gallery");

            options.Publish(c => c
                .MessagesFromNamespace("Petrichor.Services.Gallery.IntegrationMessages")
                .ToRabbitExchange("gallery-exchange"));

            options.UseFluentValidation();
        });
    }

    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheConnectionString = configuration.GetConnectionString("cache")
            ?? throw new InvalidOperationException("Connection string 'cache' not found.");

        services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(2);

                options.FactorySoftTimeout = TimeSpan.FromMilliseconds(500);
                options.FactoryHardTimeout = TimeSpan.FromSeconds(2);

                options.IsFailSafeEnabled = true;
                options.FailSafeMaxDuration = TimeSpan.FromMinutes(20);
                options.FailSafeThrottleDuration = TimeSpan.FromMinutes(1);

                options.DistributedCacheSoftTimeout = TimeSpan.FromSeconds(1);
                options.DistributedCacheHardTimeout = TimeSpan.FromSeconds(2);

                options.JitterMaxDuration = TimeSpan.FromSeconds(2);
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions()
            {
              Configuration = cacheConnectionString
            }));

        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(
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

        services.AddScoped<IAuthorizationHandler, MustBeImageUploaderOrAdminRequirementHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(GalleryPolicies.ImageUploaderOrAdmin, policy =>
                policy.AddRequirements(new MustBeImageUploaderOrAdminRequirement()));


        return services;
    }
}