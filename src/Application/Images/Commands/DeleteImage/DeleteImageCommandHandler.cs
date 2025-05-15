using Application.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Application.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler(
    IImagesRepository imagesRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteImageCommand, ErrorOr<Deleted>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Deleted>> Handle(DeleteImageCommand command, CancellationToken cancellationToken)
    {
        var image = await _imagesRepository.GetByIdAsync(command.ImageId);

        if (image is null)
        {
            return Error.NotFound(description: "Image not found");
        }

        image.DeleteImage();

        await _imagesRepository.RemoveImageAsync(image);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}