using MoreLinq;

namespace AdventOfCode2023.Solutions;

public record Day9 : Day<Day9>, IDay<Day9>
{
    public static int DayNumber => 9;

    public static int ExtrapolateRight(IEnumerable<int> sequence)
    {
        var result = 0;
        while (sequence.Any(_ => _ != 0))
        {
            result += sequence.Last();
            sequence = sequence.Pairwise((a, b) => b - a);
        }

        return result;
    }

    public static object SolvePart1(ImmutableArray<string> input) =>
        input.Select(x => x.Split(' ').Select(int.Parse))
            .Sum(ExtrapolateRight);

    public static int ExtrapolateLeft(IEnumerable<int> sequence)
    {
        IEnumerable<int> starts = [];
        while (sequence.Any(_ => _ != 0))
        {
            starts = starts.Prepend(sequence.First());
            sequence = sequence.Pairwise((a, b) => b - a);
        }

        return starts.Aggregate(0, (a, b) => b - a);
    }

    public static object SolvePart2(ImmutableArray<string> input) =>
        input.Select(x => x.Split(' ').Select(int.Parse))
            .Sum(ExtrapolateLeft);
}