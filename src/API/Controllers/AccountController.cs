using Application.Account.Queries.GetCurrentUserInfo;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
