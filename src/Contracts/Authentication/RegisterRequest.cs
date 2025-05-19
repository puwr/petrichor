namespace Contracts.Authentication;

public record RegisterRequest(string Email, string UserName, string Password);
