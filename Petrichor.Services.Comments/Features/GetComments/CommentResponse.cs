using Petrichor.Services.Comments.Common.Domain;

namespace Petrichor.Services.Comments.Features.GetComments;

public record CommentResponse
{
    public Guid Id { get; init; }
    public Guid ResourceId { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorUserName { get; init; }
    public string Message { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public static CommentResponse From(Comment comment, UserSnapshot? userSnapshot)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            ResourceId = comment.ResourceId,
            AuthorId = comment.AuthorId,
            AuthorUserName = userSnapshot?.UserName ?? "Unknown",
            Message = comment.Message,
            CreatedAtUtc = comment.CreatedAtUtc
        };
    }
}