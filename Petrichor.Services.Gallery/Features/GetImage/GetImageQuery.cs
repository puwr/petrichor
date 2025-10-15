using ErrorOr;
using MediatR;

namespace Petrichor.Services.Gallery.Features.GetImage;

public record GetImageQuery(Guid ImageId): IRequest<ErrorOr<GetImageResponse>>;