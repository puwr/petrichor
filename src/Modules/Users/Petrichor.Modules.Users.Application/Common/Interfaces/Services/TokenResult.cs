namespace Petrichor.Modules.Users.Application.Common.Interfaces.Services.Authentication;

public record TokenResult(string Token, DateTime ExpiresAt);