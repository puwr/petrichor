namespace Petrichor.Shared.Infrastructure.Inbox;

public class InboxMessage
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Content { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error { get; set; }
}