using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Features.Authentication.Login;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Tests.TestUtilities;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Login;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class LoginEndpointTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly UsersDbContext _dbContext;

    public LoginEndpointTests(ApiFactory apiFactory)
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
    public async Task Login_ReturnsNoContent()
    {
        using var client = _apiFactory.CreateClient();

        var testEmail = $"test{Random.Shared.Next(1, 10000)}@example.com";
        var testPassword = "Pa$$w0rd";

        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: testEmail,
            UserName: $"test{Random.Shared.Next(1, 10000)}",
            Password: testPassword));

        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest(
            Email: testEmail,
            Password: testPassword));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response.Headers.TryGetValues("Set-Cookie", out var loginCookies)
            .Should().BeTrue();

        var accessToken = loginCookies?.FirstOrDefault(c => c.StartsWith("ACCESS_TOKEN="));
        accessToken.Should().NotBeNull();

        var refreshToken = loginCookies?.FirstOrDefault(c => c.StartsWith("REFRESH_TOKEN="));
        refreshToken.Should().NotBeNull();

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == testEmail);
        user.Should().NotBeNull();

        WebUtility.UrlDecode(refreshToken).Should().Contain(user.RefreshToken);
    }

    [Fact]
    public async Task Login_WithInvalidData_ReturnsBadRequest()
    {
        using var client = _apiFactory.CreateClient();

        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest(
            Email: "test",
            Password: "Pa$$w0rd"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}