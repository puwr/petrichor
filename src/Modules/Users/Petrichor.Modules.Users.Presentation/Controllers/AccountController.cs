using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Modules.Shared.Presentation;
using Petrichor.Modules.Users.Application.Account.Queries.GetCurrentUserInfo;
using Petrichor.Modules.Users.Contracts.Account;

namespace Petrichor.Modules.Users.Presentation.Controllers;

public class AccountController(ISender mediator) : ApiController
{
    [HttpGet("me")]
    [EndpointSummary("Get current user")]
    [ProducesResponseType<GetCurrentUserInfoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
