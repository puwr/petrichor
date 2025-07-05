using ErrorOr;
using FluentValidation;
using Mediator;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TMessage>(message);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var validationErrors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (validationErrors.Count > 0)
        {
            return (dynamic)validationErrors.ConvertAll(
                error => Error.Validation(error.PropertyName, error.ErrorMessage)).ToList();
        }

        return await next(message, cancellationToken);
    }
}
