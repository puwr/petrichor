namespace Petrichor.Modules.Users.Contracts.Authentication;

public record LoginRequest(string Email, string Password);