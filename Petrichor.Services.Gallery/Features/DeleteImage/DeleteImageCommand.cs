using ErrorOr;
using MediatR;

namespace Petrichor.Services.Gallery.Features.DeleteImage;

public record DeleteImageCommand(Guid ImageId) : IRequest<ErrorOr<Deleted>>;