namespace AdventOfCode2023;

public interface IDay<T> where T : IDay<T>
{
    static abstract int DayNumber { get; }

    static abstract object SolvePart1(ImmutableArray<string> input);

    static virtual object SolvePart2(ImmutableArray<string> input) => "TBC";

    private static readonly ImmutableArray<string> Input = [.. File.ReadAllLines($"Input/day{T.DayNumber}.txt")];

    static Solution Solve() => new(T.SolvePart1(IDay<T>.Input), T.SolvePart2(IDay<T>.Input));
}

public abstract record Day<T> where T : IDay<T>
{
    public static Solution Solve() => IDay<T>.Solve();
}

public record Solution(object Part1, object Part2)
{
    public override string ToString() => $"Part1: {Part1}\nPart2: {Part2}";
}