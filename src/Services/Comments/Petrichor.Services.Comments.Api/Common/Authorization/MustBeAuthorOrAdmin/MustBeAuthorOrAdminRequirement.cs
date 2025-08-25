using Microsoft.AspNetCore.Authorization;

namespace Petrichor.Services.Comments.Api.Common.Authorization.MustBeAuthorOrAdmin;

public class MustBeAuthorOrAdminRequirement() : IAuthorizationRequirement
{
}