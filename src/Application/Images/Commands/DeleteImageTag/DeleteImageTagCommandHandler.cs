using Application.Common.Interfaces;
using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Commands.DeleteImageTag;

public class DeleteImageTagCommandHandler(
    IImagesRepository imagesRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteImageTagCommand, ErrorOr<Deleted>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Deleted>> Handle(DeleteImageTagCommand command, CancellationToken cancellationToken)
    {
        if (await _imagesRepository.GetByIdAsync(command.ImageId) is not Image image)
        {
            return Error.NotFound("Image not found");
        }

        var removeTagResult = image.RemoveTag(command.TagId);

        if (removeTagResult.IsError)
        {
            return removeTagResult.Errors;
        }

        await _imagesRepository.UpdateImageAsync(image);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}
