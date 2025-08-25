using ErrorOr;
using MediatR;

namespace Petrichor.Services.Comments.Api.Features.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : IRequest<ErrorOr<Deleted>>;
