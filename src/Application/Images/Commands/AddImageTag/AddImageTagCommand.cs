using ErrorOr;
using MediatR;

namespace Application.Images.Commands.AddImageTag;

public record AddImageTagCommand(Guid ImageId, string Tag): IRequest<ErrorOr<Success>>;