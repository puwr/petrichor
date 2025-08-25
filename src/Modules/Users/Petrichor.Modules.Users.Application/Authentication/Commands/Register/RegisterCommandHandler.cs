using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Users.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    IUsersDbContext dbContext,
    EventPublisher<IUsersDbContext> eventPublisher)
    : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var userExists = await dbContext.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(u => u.Email == command.Email || u.UserName == command.UserName)
            .FirstOrDefaultAsync(cancellationToken) is not null;

        if (userExists)
        {
            return Error
                .Conflict(description: "User already exists.");
        }

        var isFirstUser = !await userManager.Users.AnyAsync(cancellationToken);

        var user = new User(command.Email, command.UserName);

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            List<Error> errors = [];

            foreach (var error in result.Errors)
            {
                errors.Add(Error.Validation(
                    code: error.Code, description: error.Description));
            }

            return errors;
        }

        if (isFirstUser)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }

        eventPublisher.Publish(new UserRegisteredIntegrationEvent(
            user.Id,
            user.UserName!));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}