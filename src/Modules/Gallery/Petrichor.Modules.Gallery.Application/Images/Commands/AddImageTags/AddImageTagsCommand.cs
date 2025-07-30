using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Gallery.Application.Images.Commands.AddImageTags;

public record AddImageTagsCommand(Guid ImageId, List<string> Tags): IRequest<ErrorOr<Success>>;