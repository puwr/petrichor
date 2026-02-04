using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Services.Users.Tests.TestUtilities;
using Petrichor.TestUtilities;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Register;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class RegisterEndpointTests
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly UsersDbContext _dbContext;

    public RegisterEndpointTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    }

    [Fact]
    public async Task Register_ReturnsCreated()
    {
        using var client = _apiFactory.CreateClient();

        var testUserName = $"test{Random.Shared.Next(1, 10000)}";

        var (response, session) = await _apiFactory.Services.TrackHttpCall(
            async () => await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
                Email: $"{testUserName}@example.com",
                UserName: testUserName,
                Password: "Pa$$w0rd")));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var registeredUserId = await response.Content.ReadFromJsonAsync<Guid>();
        registeredUserId.Should().NotBeEmpty();

        var userRegisteredEvent = session.Sent.SingleMessage<UserRegisteredIntegrationEvent>();
        userRegisteredEvent.UserId.Should().Be(registeredUserId);
        userRegisteredEvent.UserName.Should().Be(testUserName);

        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == registeredUserId);
        user.Should().NotBeNull();
        user.UserName.Should().Be(testUserName);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        using var client = _apiFactory.CreateClient();

        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        var response = await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "qwerty"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WhenEmailTaken_ReturnsConflict()
    {
        using var client = _apiFactory.CreateClient();

        var testEmail = $"test{Random.Shared.Next(1, 10000)}@example.com";

        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: testEmail,
            UserName: $"test{Random.Shared.Next(1, 10000)}",
            Password: "Pa$$w0rd"));

        var registerRequest = new RegisterRequest(
            Email: testEmail,
            UserName: $"test{Random.Shared.Next(1, 10000)}",
            Password: "Pa$$w0rd");

        var response = await client.PostAsJsonAsync("/auth/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WhenUserNameTaken_ReturnsConflict()
    {
        using var client = _apiFactory.CreateClient();

        var testUserName = $"test{Random.Shared.Next(1, 10000)}";

        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"test{Random.Shared.Next(1, 10000)}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));

        var registerRequest = new RegisterRequest(
            Email: $"test{Random.Shared.Next(1, 10000)}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd");

        var response = await client.PostAsJsonAsync("/auth/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}