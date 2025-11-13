using FluentValidation;
using Petrichor.Services.Gallery.Common.Domain.Images;

namespace Petrichor.Services.Gallery.Features.UpdateImageTags;

public class UpdateImageTagsCommandValidator : AbstractValidator<UpdateImageTagsCommand>
{
    public UpdateImageTagsCommandValidator()
    {
        RuleFor(c => c.Tags)
            .Cascade(CascadeMode.Stop)
            .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
                .WithMessage("Please enter valid tags.")
            .Must(tags => tags.Count <= ImageConstants.MaxTagsPerImage)
                .WithMessage($"Too many tags. Maximum is {ImageConstants.MaxTagsPerImage} per image.");
    }
}
