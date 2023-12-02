namespace WordWeaver.Extensions;

public static class StringExtensions
{
    public static List<long> ToLongList(this string? commaSeparatedString)
    {
        if (string.IsNullOrWhiteSpace(commaSeparatedString))
        {
            return [];
        }

        var stringArray = commaSeparatedString.Split(',');
        var longList = new List<long>();

        foreach (var item in stringArray)
        {
            if (long.TryParse(item.Trim(), out var value))
            {
                longList.Add(value);
            }
            else
            {
                throw new ArgumentException($"Invalid value: {item}");
            }
        }

        return longList;
    }

    public static string ToCommaSeparatedString(this List<long>? list)
    {
        return list == null || !list.Any() 
            ? string.Empty 
            : string.Join(", ", list);
    }
}
