namespace Application.Common.Utilities;

public static class TagHelpers
{
    public static List<string> Normalize(IEnumerable<string>? tags)
        => tags?
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList()
        ?? [];
}
