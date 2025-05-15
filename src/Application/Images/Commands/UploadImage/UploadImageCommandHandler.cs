using Application.Common.Interfaces;
using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Commands.UploadImage;

public class UploadImageCommandHandler(IImagesRepository imagesRepository, IUnitOfWork unitOfWork) : IRequestHandler<UploadImageCommand, ErrorOr<Image>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Image>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        var image = new Image(command.ImagePath, command.ThumbnailPath, Guid.NewGuid());

        await _imagesRepository.AddImageAsync(image);
        await _unitOfWork.CommitChangesAsync();

        return image;
    }
}
