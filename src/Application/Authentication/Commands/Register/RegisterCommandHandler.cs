using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    UserManager<User> userManager)
    : IRequestHandler<RegisterCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(command.Email) is not null)
        {
            return Error
                .Conflict(description: "User with provided email already exists.");
        }

        if (await userManager.FindByNameAsync(command.UserName) is not null)
        {
            return Error
                .Conflict(description: "User with provided user name already exists.");
        }

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

        return Result.Success;
    }
}