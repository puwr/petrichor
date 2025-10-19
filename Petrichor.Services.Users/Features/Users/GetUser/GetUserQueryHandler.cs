using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Persistence;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Users.Features.Users.GetUser;

public class GetUserQueryHandler(IServiceScopeFactory scopeFactory, IFusionCache cache)
    : IRequestHandler<GetUserQuery, ErrorOr<GetUserResponse>>
{
    public async Task<ErrorOr<GetUserResponse>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var response = await cache.GetOrSetAsync<ErrorOr<GetUserResponse>>(
            $"user:{request.UserId}",
            async _ =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

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
            },
            token: cancellationToken);

        return response;
    }
}