using ErrorOr;
using MediatR;

namespace Petrichor.Services.Comments.Api.Features.CreateComment;

public record CreateCommentCommand(Guid AuthorId, Guid ResourceId, string Message)
    : IRequest<ErrorOr<Guid>>;