namespace Petrichor.Services.Users.Common.Services;

public record TokenResult(string Token, DateTime ExpiresAtUtc);