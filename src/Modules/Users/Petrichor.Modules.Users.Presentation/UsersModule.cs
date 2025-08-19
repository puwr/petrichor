using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Application.Common.Interfaces.Services;
using Petrichor.Modules.Users.Contracts.Authentication;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Modules.Users.Infrastructure.Persistence;
using Petrichor.Modules.Users.Infrastructure.Services;
using Petrichor.Modules.Users.Presentation;
using Petrichor.Shared.Infrastructure.Inbox;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Users.Presentation;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddAuthentication(configuration);

        services.AddControllers()
            .AddApplicationPart(typeof(UsersModule).Assembly);

        services.AddScoped<ICookieService, CookieService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "users"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertDomainOutboxMessagesInterceptor>()));

        services.AddScoped<IUsersDbContext>(provider =>
            provider.GetRequiredService<UsersDbContext>());

        services.AddHostedService<InboxBackgroundService<UsersDbContext>>();
        services.AddHostedService<OutboxBackgroudService<UsersDbContext>>();

        return services;
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
}
