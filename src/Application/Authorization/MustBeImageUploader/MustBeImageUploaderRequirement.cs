using Microsoft.AspNetCore.Authorization;

namespace Application.Authorization.MustBeImageUploader;

public class MustBeImageUploaderRequirement() : IAuthorizationRequirement
{
}