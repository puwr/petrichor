using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Outbox;

namespace Petrichor.Services.Users.Features.Authentication.Register;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    UsersDbContext dbContext,
    EventPublisher<UsersDbContext> eventPublisher)
    : IRequestHandler<RegisterCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var existingUser = await dbContext.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(u => u.Email == command.Email || u.UserName == command.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser is not null)
        {
            if (existingUser.Email == command.Email)
                return Error
                    .Conflict(description: "Email is already taken.");

            if (existingUser.UserName == command.UserName)
                return Error
                    .Conflict(description: "Username is already taken.");
        }

        var isFirstUser = !await userManager.Users.AnyAsync(cancellationToken);

        var user = User.Create(command.Email, command.UserName);

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

        return user.Id;
    }
}