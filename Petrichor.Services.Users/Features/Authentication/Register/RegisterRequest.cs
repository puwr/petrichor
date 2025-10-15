namespace Petrichor.Services.Users.Features.Authentication.Register;

public record RegisterRequest(string Email, string UserName, string Password);
