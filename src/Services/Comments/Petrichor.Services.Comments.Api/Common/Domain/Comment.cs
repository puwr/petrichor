namespace Petrichor.Services.Comments.Api.Common.Domain;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid ResourceId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Comment Create(Guid resourceId, Guid authorId, string message)
    {
        return new Comment
        {
            Id = Guid.CreateVersion7(),
            ResourceId = resourceId,
            AuthorId = authorId,
            Message = message,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}