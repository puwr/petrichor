using Microsoft.AspNetCore.Authorization;

namespace Petrichor.Services.Gallery.Common.Authorization.MustBeImageUploaderOrAdmin;

public class MustBeImageUploaderOrAdminRequirement() : IAuthorizationRequirement
{
}