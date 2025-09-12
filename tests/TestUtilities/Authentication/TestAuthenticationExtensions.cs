using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace TestUtilities.Authentication;

public static class TestAuthenticationExtensions
{
    public static void SetFakeClaims(this HttpClient client, string? userId = null, string? role = null)
    {
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, userId ?? Guid.NewGuid().ToString()},
            { ClaimTypes.Role, role ?? "" }
        };

        var token = JsonSerializer.Serialize(claims);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}