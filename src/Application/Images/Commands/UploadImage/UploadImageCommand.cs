using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Images.Commands.UploadImage;

public record UploadImageCommand(IFormFile ImageFile) : IRequest<ErrorOr<Guid>>;

