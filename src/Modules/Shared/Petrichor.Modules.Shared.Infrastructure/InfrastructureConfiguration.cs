using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Petrichor.Modules.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Shared.Infrastructure.Services.Storage.Minio;

namespace Petrichor.Modules.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static void AddInfrastructure(
        this WebApplicationBuilder builder,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.AddMinioClient("minio");

        builder.Services.AddScoped<IFileStorage, MinioFileStorage>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHostedService<MinioCreateBucketsTask>();
        }

        builder.AddMassTransitRabbitMq("rmq", configureOptions: null, configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }
        });
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