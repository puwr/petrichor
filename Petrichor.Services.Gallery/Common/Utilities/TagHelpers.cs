using System.IO.Hashing;
using System.Text;

namespace Petrichor.Services.Gallery.Common.Utilities;

public static class TagHelpers
{
    public static List<string> Normalize(IEnumerable<string>? tags)
        => tags?
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList()
        ?? [];

    public static string Hash(IEnumerable<string>? tags)
    {
        if (tags is null || !tags.Any())
        {
            return "null";
        }

        var tagsString = string.Join(',', tags.OrderBy(t => t));
        var tagsData = Encoding.UTF8.GetBytes(tagsString);

        var hashBytes = XxHash3.Hash(tagsData);
        var hashHex = Convert.ToHexStringLower(hashBytes);

        return hashHex;
    }
}