using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;

public record UploadImageCommand(IFormFile ImageFile) : IRequest<ErrorOr<Guid>>;

