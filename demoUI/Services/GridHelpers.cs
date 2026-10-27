namespace demoUI.Services;

public static class GridHelpers
{
    /// <summary>
    /// Radzen sends the sort as text such as "Name" or "Name desc".
    /// Returns a column the API accepts (from <paramref name="allowed"/>) and "asc" or "desc".
    /// </summary>
    public static (string SortBy, string SortOrder) ParseSort(
        string? orderBy, IEnumerable<string> allowed, string defaultColumn = "Id")
    {
        var sortBy = defaultColumn;
        var sortOrder = "asc";

        if (string.IsNullOrWhiteSpace(orderBy))
            return (sortBy, sortOrder);

        var allowedList = allowed.ToList();

        // Only the first sort is used, the API sorts by one column
        var first = orderBy.Split(',')[0].Trim();
        var parts = first.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return (sortBy, sortOrder);

        // Strip wrappers such as brackets or quotes
        var column = new string(parts[0].Where(char.IsLetterOrDigit).ToArray());

        var match = allowedList.FirstOrDefault(c => c.Equals(column, StringComparison.OrdinalIgnoreCase));

        // Some versions wrap the property as np(Name)
        if (match is null && column.StartsWith("np", StringComparison.OrdinalIgnoreCase) && column.Length > 2)
        {
            var unwrapped = column[2..];
            match = allowedList.FirstOrDefault(c => c.Equals(unwrapped, StringComparison.OrdinalIgnoreCase));
        }

        if (match is null)
            return (sortBy, sortOrder);

        sortBy = match;
        if (parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
            sortOrder = "desc";

        return (sortBy, sortOrder);
    }
}
