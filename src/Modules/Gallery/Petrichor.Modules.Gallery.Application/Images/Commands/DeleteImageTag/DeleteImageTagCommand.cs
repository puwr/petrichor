using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImageTag;

public record DeleteImageTagCommand(Guid ImageId, Guid TagId) : IRequest<ErrorOr<Deleted>>;