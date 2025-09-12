using Microsoft.AspNetCore.Authorization;

namespace Petrichor.Modules.Gallery.Infrastructure.Authorization.MustBeImageUploaderOrAdmin;

public class MustBeImageUploaderOrAdminRequirement() : IAuthorizationRequirement
{
}