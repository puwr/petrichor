using FluentValidation;

namespace Petrichor.Services.Comments.Features.CreateComment;

public class CreateCommentCommandValidator
    : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(c => c.ResourceId)
            .NotEmpty();

        RuleFor(c => c.Message)
            .Must(message => !string.IsNullOrEmpty(message?.Trim()))
                .WithMessage("Comment message is required.")
            .MaximumLength(1000);
    }
}