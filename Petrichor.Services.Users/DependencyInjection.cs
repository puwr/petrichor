using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Services.Users.Common.Authorization;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Common.Services;
using Petrichor.Shared.Settings;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.MemoryPack;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
namespace Petrichor.Services.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found.");

        services.AddDbContext<UsersDbContext>(options =>
            options
                .UseNpgsql(
                    databaseConnectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "users"))
                .UseSnakeCaseNamingConvention(),
                optionsLifetime: ServiceLifetime.Singleton);

        services.AddPooledDbContextFactory<UsersDbContext>(options =>
            options
                .UseNpgsql(
                    databaseConnectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "users"))
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
            options.UseMemoryPackSerialization();

            options.PersistMessagesWithPostgresql(databaseConnectionString, "users");
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.UseDurableLocalQueues();
            options.Policies.UseDurableInboxOnAllListeners();
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();
            options.Policies.AutoApplyTransactions();

            options.UseRabbitMq(rmqConnectionString)
                .AutoProvision();

            options.Publish(c => c
                .MessagesFromNamespace("Petrichor.Services.Users.IntegrationMessages")
                .ToRabbitExchange("users-exchange"));

            options.UseFluentValidation();
        });
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(
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

        services.AddAuthorizationBuilder()
            .AddPolicy(UsersPolicies.AdminOnly, policy => policy.RequireRole("Admin"));

        services.AddHostedService<RolesSeeder>();

        services.AddScoped<ICookieService, CookieService>();

        return services;
    }
}