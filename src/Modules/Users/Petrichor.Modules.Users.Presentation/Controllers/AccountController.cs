using MediatR;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared.Presentation;
using Petrichor.Modules.Users.Application.Account.Queries.GetCurrentUserInfo;

namespace Petrichor.Modules.Users.Presentation.Controllers;

public class AccountController(ISender mediator) : ApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var query = new GetCurrentUserInfoQuery();

        var getMeResult = await mediator.Send(query);

        return getMeResult.Match(
            Ok,
            Problem
        );
    }
}
