using Application.Common.Interfaces;
using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.ListImages;

public class ListImagesQueryHandler(IImagesRepository imagesRepository) : IRequestHandler<ListImagesQuery, ErrorOr<List<Image>>>
{
    private readonly IImagesRepository _imagesRepository = imagesRepository;

    public async Task<ErrorOr<List<Image>>> Handle(ListImagesQuery request, CancellationToken cancellationToken)
    {
        return await _imagesRepository.ListAsync();
    }
}
