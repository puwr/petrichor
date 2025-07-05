using System.Net.Http.Json;
using Application.Common.Interfaces.Services.Storage;
using Contracts.Authentication;
using Domain.Users;
using Infrastructure.Persistence;
using Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestUtilities.Database;

namespace API.Tests.TestCommon;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private TestDatabase _testDatabase;
    public HttpClient HttpClient { get; }
    private IServiceScope _scope;
    private readonly List<string> _cookies = [];
    public readonly string TestDataFolder = Path
        .Combine(Path.GetTempPath(), $"{Guid.NewGuid()}");

    public ApiFactory()
    {
        HttpClient = CreateClient();
    }

    public async Task InitializeAsync()
    {
        var loginResponse = await HttpClient.PostAsJsonAsync(
            "api/v1/auth/login",
            new LoginRequest("test@test.test", "Pa$$w0rd!"));

        _cookies.AddRange(loginResponse.Headers.GetValues("Set-Cookie"));

        HttpClient.DefaultRequestHeaders.Add("Cookie", string.Join("; ", _cookies));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _testDatabase = TestDatabase.CreateDatabase();

        builder.ConfigureTestServices(async services =>
        {
            services
                .RemoveAll<DbContextOptions<PetrichorDbContext>>()
                .AddDbContext<PetrichorDbContext>(
                    (options) => options.UseSqlite(_testDatabase.Connection));

            services.Replace(
                ServiceDescriptor
                    .Scoped<IFileStorage>(_ => new LocalFileStorage(TestDataFolder)));

            using var scope = services.BuildServiceProvider().CreateScope();
            var useManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            await useManager.CreateAsync(new User("test@test.test", "testUser"), "Pa$$w0rd!");
        });
    }

    public IDisposable AsAnonymous()
    {
        HttpClient.DefaultRequestHeaders.Remove("Cookie");
        return new CookieRestorer(this);
    }

    public PetrichorDbContext GetDbContext()
    {
        _scope?.Dispose();
        _scope = Services.CreateScope();
        return _scope.ServiceProvider.GetRequiredService<PetrichorDbContext>();
    }

    public void ResetDatabase()
    {
        _testDatabase.ResetDatabase();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _testDatabase.Dispose();
        _scope?.Dispose();

        if (Directory.Exists(TestDataFolder))
        {
            Directory.Delete(TestDataFolder, recursive: true);
        }
    }

    private class CookieRestorer(ApiFactory factory) : IDisposable
    {
        public void Dispose()
        {
            factory.HttpClient.DefaultRequestHeaders.Add("Cookie", string.Join("; ", factory._cookies));
        }
    }
}