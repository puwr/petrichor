using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploader;

namespace Petrichor.Modules.Gallery.Infrastructure.Authorization;

public class GalleryAuthorizationConfiguration : IPostConfigureOptions<AuthorizationOptions>
{
    public void PostConfigure(string? name, AuthorizationOptions options)
    {
        options.AddPolicy(GalleryPolicies.ImageUploader, policy =>
            {
                policy.AddRequirements(new MustBeImageUploaderRequirement());
            });
    }

}
