using System.Text;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Services.Comments.Api.Common.Authorization;
using Petrichor.Services.Comments.Api.Common.Authorization.MustBeAuthorOrAdmin;
using Petrichor.Services.Comments.Api.Common.Extensions;
using Petrichor.Services.Comments.Api.Common.Inbox;
using Petrichor.Services.Comments.Api.Common.Persistence;
using Petrichor.Services.Comments.Api.Common.Settings;
using Petrichor.Shared.Behaviors;
using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Events;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.IntegrationEvents;
using Petrichor.Shared.Outbox;

namespace Petrichor.Services.Comments.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddAuthentication(configuration);

        services.AddEndpoints();

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorization();

        services.AddDbContext<CommentsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "comments"))
            .UseSnakeCaseNamingConvention());

        services.AddMassTransit(configure =>
        {
            configure.AddConsumer<IntegrationEventConsumer<UserRegisteredIntegrationEvent>>();
            configure.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();

            configure.SetKebabCaseEndpointNameFormatter();

            var rabbitMqSettings = new RabbitMqSettings();
            configuration.Bind(RabbitMqSettings.Key, rabbitMqSettings);

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqSettings.Host), h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<EventPublisher<CommentsDbContext>>();

        services.AddDomainEvents();
        services.AddIntegrationEvents();

        services.AddHostedService<InboxBackgroundService<CommentsDbContext>>();
        services.AddHostedService<OutboxBackgroudService<CommentsDbContext>>();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, MustBeAuthorOrAdminRequirementHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(CommentsPolicies.AuthorOrAdmin, policy =>
                policy.AddRequirements(new MustBeAuthorOrAdminRequirement()));

        return services;
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

    private static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddScoped<DomainEventDispatcher<CommentsDbContext>>();

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
        services.AddScoped<IntegrationEventDispatcher<CommentsDbContext>>();

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