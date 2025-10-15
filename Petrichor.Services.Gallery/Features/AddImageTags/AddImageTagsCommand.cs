using ErrorOr;
using MediatR;

namespace Petrichor.Services.Gallery.Features.AddImageTags;

public record AddImageTagsCommand(Guid ImageId, List<string> Tags): IRequest<ErrorOr<Success>>;