using ErrorOr;
using MediatR;

namespace Application.Images.Commands.AddImageTags;

public record AddImageTagsCommand(Guid ImageId, List<string> Tags): IRequest<ErrorOr<Success>>;