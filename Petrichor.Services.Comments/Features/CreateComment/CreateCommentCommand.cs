namespace Petrichor.Services.Comments.Features.CreateComment;

public record CreateCommentCommand(Guid AuthorId, Guid ResourceId, string Message);
