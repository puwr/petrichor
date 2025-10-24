using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Petrichor.Services.Users.Features.Account.GetCurrentUserInfo;
using Petrichor.Services.Users.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Users.Tests.Features.Account.GetCurrentUserInfo;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetCurrentUserInfoEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetCurrentUserInfo_ReturnsOk()
    {
        using var client = apiFactory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, Guid.NewGuid() },
            { JwtRegisteredClaimNames.Email, "test@example.com" },
            { JwtRegisteredClaimNames.UniqueName, "test" },
            { ClaimTypes.Role, "Admin" }
        };
        client.SetFakeClaims(claims);

        var response = await client.GetAsync("/account/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userInfo = await response.Content.ReadFromJsonAsync<GetCurrentUserInfoResponse>();
        userInfo.Should().NotBeNull();
        userInfo.Should().BeEquivalentTo(new GetCurrentUserInfoResponse(
            Id: claims[JwtRegisteredClaimNames.Sub].ToString()!,
            Email: claims[JwtRegisteredClaimNames.Email].ToString()!,
            UserName: claims[JwtRegisteredClaimNames.UniqueName].ToString()!,
            Roles: [claims[ClaimTypes.Role].ToString()!]));
    }

    [Fact]
    public async Task GetCurrentUserInfo_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.GetAsync("/account/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}