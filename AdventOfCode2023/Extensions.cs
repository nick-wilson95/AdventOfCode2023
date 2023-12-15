namespace AdventOfCode2023;

public static class Extensions
{
    public static int ValueOrZero<T>(this Dictionary<T, int> dict, T key) where T : notnull
        => dict.TryGetValue(key, out var value) ? value : 0;

    public static int? ParseDigit(this char c) => c.ToString().ParseInt();

    public static int? ParseInt(this string str) => int.TryParse(str, out var result) ? result : null;
}
