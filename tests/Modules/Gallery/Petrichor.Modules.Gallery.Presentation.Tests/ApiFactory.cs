using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Petrichor.Api;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using Petrichor.Modules.Users.Contracts.Authentication;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Shared.Infrastructure.Services.Storage;
using Testcontainers.PostgreSql;

namespace Petrichor.Modules.Gallery.Presentation.Tests;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("petrichor")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private IServiceScope _scope;
    public HttpClient AnonymousClient { get; private set; }
    public HttpClient AuthenticatedClient { get; private set; }
    public GalleryDbContext GalleryDbContext { get; private set; }
    public readonly string TestDataFolder = Path
        .Combine(Path.GetTempPath(), $"{Guid.NewGuid()}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.Replace(
                ServiceDescriptor
                    .Scoped<IFileStorage>(_ => new LocalFileStorage(TestDataFolder)));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _scope = Services.CreateScope();

        AnonymousClient = CreateDefaultClient();
        AuthenticatedClient = await CreateAuthenticatedClient();

        GalleryDbContext = _scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
    }

    private async Task<HttpClient> CreateAuthenticatedClient()
    {
        var authenticatedClient = CreateClient();

        var useManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await useManager.CreateAsync(new User("test@test.test", "test"), "Pa$$w0rd");

        var loginResponse = await authenticatedClient.PostAsJsonAsync(
            "api/v1/auth/login",
            new LoginRequest("test@test.test", "Pa$$w0rd"));

        loginResponse.EnsureSuccessStatusCode();

        var cookies = loginResponse.Headers.GetValues("Set-Cookie");
        authenticatedClient.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookies));

        return authenticatedClient;
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
        _scope.Dispose();

        if (Directory.Exists(TestDataFolder))
        {
            Directory.Delete(TestDataFolder, recursive: true);
        }
    }
}