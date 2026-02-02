using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Features.Authentication.Login;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Tests.TestUtilities;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Logout;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class LogoutEndpointTests: IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly UsersDbContext _dbContext;

    public LogoutEndpointTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Logout_WithRefreshTokenCookie_ReturnsNoContent()
    {
        using var client = _apiFactory.CreateClient();

        var testEmail = $"test{Random.Shared.Next(1, 10000)}@example.com";
        var testPassword = "Pa$$w0rd";

        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: testEmail,
            UserName: $"test{Random.Shared.Next(1, 10000)}",
            Password: testPassword));
        var loginResponse = await client.PostAsJsonAsync("/auth/login", new LoginRequest(
            Email: testEmail,
            Password: testPassword));
        loginResponse.Headers.TryGetValues("Set-Cookie", out var loginCookies)
            .Should().BeTrue();
        var refreshTokenCookie = loginCookies?
            .FirstOrDefault(c => c.StartsWith("REFRESH_TOKEN="));
        refreshTokenCookie.Should().NotBeNull();

        client.DefaultRequestHeaders.Add("Cookie", refreshTokenCookie);

        var response = await client.PostAsync("/auth/logout", null);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Set-Cookie", out var logoutCookies)
            .Should().BeTrue();

        var deleteAccessTokenCookie = logoutCookies?
            .FirstOrDefault(c => c.StartsWith("ACCESS_TOKEN=;"));
        deleteAccessTokenCookie.Should().NotBeNull();

        var deleteRefreshTokenCookie = logoutCookies?
            .FirstOrDefault(c => c.StartsWith("REFRESH_TOKEN=;"));
        deleteRefreshTokenCookie.Should().NotBeNull();

        var refreshToken = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.User.Email == testEmail);
        refreshToken.Should().BeNull();
    }

    [Fact]
    public async Task Logout_WithNoRefreshTokenCookie_ReturnsNoContent()
    {
        using var client = _apiFactory.CreateClient();

        var response = await client.PostAsync("/auth/logout", null);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Set-Cookie", out var logoutCookies)
            .Should().BeTrue();

        var deleteAccessTokenCookie = logoutCookies?
            .FirstOrDefault(c => c.StartsWith("ACCESS_TOKEN=;"));
        deleteAccessTokenCookie.Should().NotBeNull();

        var deleteRefreshTokenCookie = logoutCookies?
            .FirstOrDefault(c => c.StartsWith("REFRESH_TOKEN=;"));
        deleteRefreshTokenCookie.Should().NotBeNull();
    }
}