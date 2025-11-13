using ErrorOr;
using MediatR;

namespace Petrichor.Services.Gallery.Features.UpdateImageTags;

public record UpdateImageTagsCommand(Guid ImageId, List<string> Tags): IRequest<ErrorOr<Success>>;