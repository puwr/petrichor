using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Api;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using TestUtilities.Authentication;

namespace Petrichor.Modules.Gallery.Presentation.Tests;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("petrichor")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private readonly RabbitMqContainer _rmqContainer = new RabbitMqBuilder().Build();
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio:latest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:rmq", _rmqContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:minio",
            $"Endpoint={_minioContainer.GetConnectionString()};AccessKey=minioadmin;SecretKey=minioadmin");

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => {});

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                    .AddAuthenticationSchemes("Test")
                    .Build();
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rmqContainer.StartAsync();
        await _minioContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _rmqContainer.DisposeAsync();
        await _minioContainer.DisposeAsync();
    }
}