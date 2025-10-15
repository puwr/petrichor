using ErrorOr;
using MediatR;

namespace Petrichor.Services.Gallery.Features.DeleteImageTag;

public record DeleteImageTagCommand(Guid ImageId, Guid TagId) : IRequest<ErrorOr<Deleted>>;