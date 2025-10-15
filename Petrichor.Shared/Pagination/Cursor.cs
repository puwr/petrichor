namespace Petrichor.Shared.Pagination;

public record Cursor
{
    public static string Encode(Guid lastId) => lastId.ToString();

    public static Guid? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        return Guid.TryParse(cursor, out Guid id) ? id : null;
    }
}
