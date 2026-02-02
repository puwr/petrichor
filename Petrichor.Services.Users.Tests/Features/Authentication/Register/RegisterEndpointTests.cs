using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Tests.TestUtilities;

namespace Petrichor.Services.Users.Tests.Features.Authentication.Register;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class RegisterEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task Register_ReturnsCreated()
    {
        using var client = apiFactory.CreateClient();

        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        var response = await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var registeredUserId = await response.Content.ReadFromJsonAsync<Guid>();
        registeredUserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        using var client = apiFactory.CreateClient();

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
        using var client = apiFactory.CreateClient();

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
        using var client = apiFactory.CreateClient();

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