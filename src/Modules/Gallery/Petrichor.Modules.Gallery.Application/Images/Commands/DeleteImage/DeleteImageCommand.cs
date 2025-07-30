using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImage;

public record DeleteImageCommand(Guid ImageId) : IRequest<ErrorOr<Deleted>>;