using Application.Common.Interfaces;
using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.GetImage;

public class GetImageQueryHandler(IImagesRepository imagesRepository) : IRequestHandler<GetImageQuery, ErrorOr<Image>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;

    public async Task<ErrorOr<Image>> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        if (await _imagesRepository.GetByIdAsync(request.ImageId) is not Image image)
        {
            return Error.NotFound("Image not found");
        }

        return image;
    }
}
