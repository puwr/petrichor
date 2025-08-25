namespace Petrichor.Services.Comments.Api.Features.CreateComment;

public record CreateCommentRequest(Guid ResourceId, string Message);
