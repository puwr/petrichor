using ErrorOr;
using Mediator;

namespace Application.Images.Commands.DeleteImageTag;

public record DeleteImageTagCommand(Guid ImageId, Guid TagId) : IRequest<ErrorOr<Deleted>>;