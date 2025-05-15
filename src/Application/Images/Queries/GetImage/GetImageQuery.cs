using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.GetImage;

public record GetImageQuery(Guid ImageId): IRequest<ErrorOr<Image>>;