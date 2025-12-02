using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.TestUtilities.Authentication;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Wolverine;

namespace Petrichor.Services.Gallery.Tests.TestUtilities;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private readonly RabbitMqContainer _rmqContainer = new RabbitMqBuilder().Build();
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:cache", _redisContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:rmq", _rmqContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:minio",
            $"Endpoint={_minioContainer.GetConnectionString()};AccessKey=minioadmin;SecretKey=minioadmin");

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => {});

            services.RunWolverineInSoloMode();
            services.DisableAllExternalWolverineTransports();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _rmqContainer.StartAsync();
        await _minioContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
        await _rmqContainer.DisposeAsync();
        await _minioContainer.DisposeAsync();
    }
}