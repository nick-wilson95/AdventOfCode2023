namespace AdventOfCode2023;

public static class DictionaryExtensions
{
    public static int ValueOrZero<T>(this Dictionary<T, int> dict, T key) where T : notnull
        => dict.TryGetValue(key, out var value) ? value : 0;
}
