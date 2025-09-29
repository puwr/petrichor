using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;
using Petrichor.Modules.Users.Application.Users.Queries.GetUser;
using Petrichor.Modules.Users.Application.Users.Queries.ListUsers;
using Petrichor.Modules.Users.Contracts.Users;
using Petrichor.Modules.Shared.Presentation;
using Petrichor.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Petrichor.Modules.Users.Contracts.Account;

namespace Petrichor.Modules.Users.Presentation.Controllers;

public class UsersController(ISender mediator) : ApiController
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [EndpointSummary("List users")]
    [ProducesResponseType<PagedResponse<ListUsersResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [EndpointSummary("Get user")]
    [ProducesResponseType<GetUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [EndpointSummary("Delete user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(Guid userId, DeleteUserRequest request)
    {
        var command = new DeleteUserCommand(userId, request.DeleteUploadedImages);

        var deleteUserResult = await mediator.Send(command);

        return deleteUserResult.Match(
            _ => NoContent(),
            Problem
        );
    }
}