using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Contracts.Account;

namespace Petrichor.Modules.Users.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUsersDbContext dbContext)
    : IRequestHandler<GetUserQuery, ErrorOr<GetUserResponse>>
{
    public async Task<ErrorOr<GetUserResponse>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        return new GetUserResponse(Id: user.Id, UserName: user.UserName!);
    }

}
