using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Shared.Infrastructure.Outbox;
using Petrichor.Shared.Infrastructure.Services.Storage.Minio;
using Petrichor.Shared.Infrastructure.Settings;

namespace Petrichor.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddPersistence(configuration);

        services.TryAddSingleton<InsertDomainOutboxMessagesInterceptor>();

        services.AddMassTransit(configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

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

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var minioSettings = new MinioSettings();
        configuration.Bind(MinioSettings.Key, minioSettings);

        services.AddMinio(configure => configure
            .WithEndpoint(minioSettings.Endpoint)
            .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
            .WithSSL(minioSettings.UseSsl)
            .Build());

        services.AddScoped<IFileStorage, MinioFileStorage>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructureMiddleware(this IApplicationBuilder builder)
    {
        var minio = builder.ApplicationServices.GetRequiredService<IMinioClient>();

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new MinioFileProvider(minio, "uploads"),
            RequestPath = "/uploads"
        });

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new MinioFileProvider(minio, "thumbs"),
            RequestPath = "/thumbs"
        });

        return builder;
    }
}