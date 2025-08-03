using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;
using Petrichor.Modules.Users.Application.Users.Queries.GetUser;
using Petrichor.Modules.Users.Application.Users.Queries.ListUsers;
using Petrichor.Shared.Contracts.Pagination;
using Petrichor.Shared.Presentation;

namespace Petrichor.Modules.Users.Presentation.Controllers;

public class UsersController(ISender mediator) : ApiController
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ListUsers([FromQuery(Name = "page")] int pageNumber = 1)
    {
        var pagination = new PaginationParameters(pageNumber);

        var query = new ListUsersQuery(pagination);

        var listUsersResult = await mediator.Send(query);

        return listUsersResult.Match(
            Ok,
            Problem
        );
    }

    [AllowAnonymous]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var query = new GetUserQuery(userId);

        var getUserResult = await mediator.Send(query);

        return getUserResult.Match(
            Ok,
            Problem
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var command = new DeleteUserCommand(userId);

        var deleteUserResult = await mediator.Send(command);

        return deleteUserResult.Match(
            _ => NoContent(),
            Problem
        );
    }
}