using ErrorOr;
using MediatR;
using Petrichor.Modules.Gallery.Contracts.Images;

namespace Petrichor.Modules.Gallery.Application.Images.Queries.GetImage;

public record GetImageQuery(Guid ImageId): IRequest<ErrorOr<ImageResponse>>;