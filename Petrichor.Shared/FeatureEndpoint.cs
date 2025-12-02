using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Petrichor.Shared;

public abstract class FeatureEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder);

    protected static IResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    protected static IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    private static IResult ValidationProblem(List<Error> errors)
    {
        var errorsDictionary = errors.ToDictionary(
            error => error.Code,
            error => new[] { error.Description }
        );

        return Results.ValidationProblem(errors: errorsDictionary);
    }
}