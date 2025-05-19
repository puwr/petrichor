using Application.Common.Interfaces;
using Domain.Images;
using Domain.Tags;
using ErrorOr;
using MediatR;

namespace Application.Images.Commands.AddImageTag;

public class AddImageTagCommandHandler(
    IImagesRepository imagesRepository, 
    ITagsRepository tagsRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<AddImageTagCommand, ErrorOr<Success>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;
    private readonly ITagsRepository _tagsRepository = tagsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Success>> Handle(AddImageTagCommand command, CancellationToken cancellationToken)
    {
        if (await _imagesRepository.GetByIdAsync(command.ImageId) is not Image image)
        {
            return Error.NotFound("Image not found.");
        }

        var tag = await _tagsRepository.GetByNameAsync(command.Tag) ?? new Tag(command.Tag);

        var addTagResult = image.AddTag(tag);

        if (addTagResult.IsError)
        {
            return addTagResult.Errors;
        }

        await _imagesRepository.UpdateImageAsync(image);
        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}
