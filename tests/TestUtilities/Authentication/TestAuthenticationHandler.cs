using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestUtilities.Authentication;

public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token)) return Task.FromResult(AuthenticateResult.NoResult());

        try
        {
            var claimsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(token)!;

            var claims = new List<Claim>();

            foreach (var claim in claimsDict)
            {
                claims.Add(new Claim(claim.Key, claim.Value.ToString()!));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }
    }
}