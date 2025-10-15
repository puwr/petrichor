namespace Petrichor.Services.Comments.Features.CreateComment;

public record CreateCommentRequest(Guid ResourceId, string Message);
