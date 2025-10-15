using Microsoft.AspNetCore.Authorization;

namespace Petrichor.Services.Comments.Common.Authorization.MustBeAuthorOrAdmin;

public class MustBeAuthorOrAdminRequirement() : IAuthorizationRequirement
{
}