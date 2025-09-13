using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Comments.Api;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using TestUtilities.Authentication;

namespace Petrichor.Services.Comments.Api.Tests;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("petrichor")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private readonly RabbitMqContainer _rmqContainer = new RabbitMqBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:rmq", _rmqContainer.GetConnectionString());

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => {});
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rmqContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _rmqContainer.DisposeAsync();
    }
}