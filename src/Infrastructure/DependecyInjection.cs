using System.Text;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Authentication;
using Application.Common.Interfaces.Services.Images;
using Application.Common.Interfaces.Services.Storage;
using Contracts.Authentication;
using Domain.Users;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.Images;
using Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddPersistence()
            .AddAuthentication(configuration)
            .AddAuthorization();

        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        services.AddScoped<IImageMetadataProvider, ImageMetadataProvider>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<PetrichorDbContext>(options =>
            options.UseSqlite("Data Source = Petrichor.db"));

        services.AddScoped<IPetrichorDbContext>(provider =>
            provider.GetRequiredService<PetrichorDbContext>());

        services.AddScoped<IFileStorage, LocalFileStorage>();

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
        }).AddEntityFrameworkStores<PetrichorDbContext>();

        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.JwtSettingsKey, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();

        services
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
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