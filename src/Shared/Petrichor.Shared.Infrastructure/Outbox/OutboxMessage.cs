using System.Text.Json;
using Petrichor.Shared.Domain.Common;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; }
    public string Content { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error { get; set; }
}