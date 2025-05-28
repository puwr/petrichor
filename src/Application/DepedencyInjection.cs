using Application.Authorization;
using Application.Authorization.MustBeImageUploader;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
            options.RegisterServicesFromAssemblyContaining(
                typeof(DependencyInjection)));

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, MustBeImageUploaderHandler>();

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(options.DefaultPolicy)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AuthorizationPolicies.ImageUploader, policy =>
            {
                policy.AddRequirements(new MustBeImageUploaderRequirement());
            });

        });
        return services;
    }
}