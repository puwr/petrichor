using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Services.Comments.Common.Authorization;
using Petrichor.Services.Comments.Common.Authorization.MustBeAuthorOrAdmin;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Shared.Settings;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Petrichor.Services.Comments;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.");

        services.AddDbContext<CommentsDbContext>(options =>
            options.UseNpgsql(
                databaseConnectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "comments"))
            .UseSnakeCaseNamingConvention(), optionsLifetime: ServiceLifetime.Singleton);

        services.AddPooledDbContextFactory<CommentsDbContext>(options =>
            options.UseNpgsql(
                databaseConnectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "comments"))
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
            options.PersistMessagesWithPostgresql(databaseConnectionString, "comments");
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.UseDurableLocalQueues();
            options.Policies.UseDurableInboxOnAllListeners();
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();
            options.Policies.AutoApplyTransactions();

            options.UseRabbitMq(rmqConnectionString)
                .AutoProvision()
                .BindExchange("users-exchange").ToQueue("users-comments")
                .BindExchange("gallery-exchange").ToQueue("gallery-comments");

            options.ListenToRabbitQueue("users-comments");
            options.ListenToRabbitQueue("gallery-comments");

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

        services.AddScoped<IAuthorizationHandler, MustBeAuthorOrAdminRequirementHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(CommentsPolicies.AuthorOrAdmin, policy =>
                policy.AddRequirements(new MustBeAuthorOrAdminRequirement()));

        return services;
    }
}