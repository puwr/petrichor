using FluentValidation;
using Petrichor.Modules.Gallery.Domain.Images;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.AddImageTags;

public class AddImageTagsCommandValidator : AbstractValidator<AddImageTagsCommand>
{
    public AddImageTagsCommandValidator()
    {
        RuleFor(c => c.Tags)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Please enter at least one tag.")
            .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
                .WithMessage("Please enter valid tags.")
            .Must(tags => tags.Count <= ImageConstants.MaxTagsPerImage)
                .WithMessage($"Too many tags. Maximum is {ImageConstants.MaxTagsPerImage} per image.");
    }
}
