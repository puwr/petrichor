using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Commands.UploadImage;

public record UploadImageCommand(string ImagePath, string ThumbnailPath, Guid UserId) : IRequest<ErrorOr<Image>>;

