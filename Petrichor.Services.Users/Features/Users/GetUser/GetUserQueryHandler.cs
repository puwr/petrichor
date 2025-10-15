using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Persistence;

namespace Petrichor.Services.Users.Features.Users.GetUser;

public class GetUserQueryHandler(UsersDbContext dbContext)
    : IRequestHandler<GetUserQuery, ErrorOr<GetUserResponse>>
{
    public async Task<ErrorOr<GetUserResponse>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        if (user.IsDeleted)
        {
            return new GetUserResponse(Id: user.Id, UserName: "Deleted User");
        }

        return new GetUserResponse(Id: user.Id, UserName: user.UserName!);
    }
}