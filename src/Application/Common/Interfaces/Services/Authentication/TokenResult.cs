namespace Application.Common.Interfaces.Services.Authentication;

public record TokenResult(string Token, DateTime ExpiresAt);