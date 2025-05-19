using System.Text.Json.Serialization;
using Application.Authorization;
using Application.Authorization.MustBeImageUploader;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddHttpContextAccessor()
            .AddProblemDetails()
            .AddAuthorization();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AuthorizationPolicies.ImageUploader, new AuthorizationPolicyBuilder()
                .AddRequirements(new MustBeImageUploaderRequirement())
                .Build());

        });

        return services;
    }
}