using ErrorOr;
using MediatR;

namespace Petrichor.Services.Comments.Features.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : IRequest<ErrorOr<Deleted>>;
