using Microsoft.AspNetCore.Authorization;

namespace Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploader;

public class MustBeImageUploaderRequirement() : IAuthorizationRequirement
{
}