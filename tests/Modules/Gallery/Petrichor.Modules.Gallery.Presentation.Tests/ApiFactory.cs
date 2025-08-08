using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel.Args;
using Petrichor.Api;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using Petrichor.Modules.Users.Contracts.Authentication;
using Petrichor.Modules.Users.Domain.Users;
using Testcontainers.Minio;
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
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio:latest")
        .WithPortBinding(9000, true)
        .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
        .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
        .Build();
    private IServiceScope _scope;
    public HttpClient AnonymousClient { get; private set; }
    public HttpClient AuthenticatedClient { get; private set; }
    public GalleryDbContext GalleryDbContext { get; private set; }
    public IMinioClient MinioClient { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Minio:Endpoint", $"localhost:{_minioContainer.GetMappedPublicPort(9000)}");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _minioContainer.StartAsync();

        _scope = Services.CreateScope();

        AnonymousClient = CreateDefaultClient();
        AuthenticatedClient = await CreateAuthenticatedClient();

        GalleryDbContext = _scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
        MinioClient = _scope.ServiceProvider.GetRequiredService<IMinioClient>();

        await MinioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket("uploads"));
        await MinioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket("thumbs"));
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
        await _minioContainer.DisposeAsync();
        _scope.Dispose();
    }
}