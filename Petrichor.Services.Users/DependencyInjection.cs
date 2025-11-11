using System.Text;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Extensions;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Common.Services;
using Petrichor.Shared.Behaviors;
using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Events;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.IntegrationEvents;
using Petrichor.Shared.Outbox;
using Petrichor.Shared.Settings;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Petrichor.Services.Users;

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

    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(builder.Configuration);

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(UsersPolicies.AdminOnly, policy => policy.RequireRole("Admin"));

        builder.Services.AddDbContext<UsersDbContext>(options =>
            options
                .UseNpgsql(
                    builder.Configuration.GetConnectionString("database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "users"))
                .UseSnakeCaseNamingConvention());

        builder.Services.AddFusionCache()
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
                Configuration = builder.Configuration.GetConnectionString("cache")
            }));

        builder.Services.AddMassTransit(configure =>
        {
            configure.DisableUsageTelemetry();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("rmq"));
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(prefix: "users"));
            });
        });

        builder.Services.AddScoped<EventPublisher<UsersDbContext>>();

        builder.Services.AddDomainEvents();
        builder.Services.AddIntegrationEvents();

        builder.Services.AddHostedService<InboxBackgroundService<UsersDbContext>>();
        builder.Services.AddHostedService<OutboxBackgroudService<UsersDbContext>>();

        builder.Services.AddScoped<ICookieService, CookieService>();
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
        }).AddEntityFrameworkStores<UsersDbContext>();

        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Key, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));

        services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddScoped<DomainEventDispatcher<UsersDbContext>>();

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
        services.AddScoped<IntegrationEventDispatcher<UsersDbContext>>();

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