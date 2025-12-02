using Petrichor.Services.Comments.Common.Domain;

namespace Petrichor.Services.Comments.Features.GetComments;

public record GetCommentsResponse
{
    public Guid Id { get; init; }
    public Guid ResourceId { get; init; }
    public string Author { get; init; }
    public string Message { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public static GetCommentsResponse From(Comment comment, UserSnapshot? userSnapshot)
    {
        return new GetCommentsResponse
        {
            Id = comment.Id,
            ResourceId = comment.ResourceId,
            Author = userSnapshot?.UserName ?? "Deleted",
            Message = comment.Message,
            CreatedAtUtc = comment.CreatedAtUtc
        };
    }
}