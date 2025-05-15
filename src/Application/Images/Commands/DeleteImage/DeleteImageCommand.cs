using ErrorOr;
using MediatR;

namespace Application.Images.Commands.DeleteImage;

public record DeleteImageCommand(Guid ImageId) : IRequest<ErrorOr<Deleted>>;