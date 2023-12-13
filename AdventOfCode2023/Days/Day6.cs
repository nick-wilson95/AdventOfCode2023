using static System.StringSplitOptions;

namespace AdventOfCode2023.Solutions;

public record Day6 : Day<Day6>, IDay<Day6>
{
    public static int DayNumber => 6;

    private record Race(long Time, long Distance)
    {
        public int NumWaysToWin()
        {
            var (T, D) = (Time, Distance);
            var det = Math.Sqrt(Math.Pow(T, 2) - 4 * D);
            var lowBound = (T - det) / 2;
            var highBound = (T + det) / 2;

            return (int)Math.Ceiling(highBound) - (int)lowBound - 1;
        }
    }

    public static object SolvePart1(ImmutableArray<string> input) => input
        .Select(_ => _.Split(' ', RemoveEmptyEntries)[1..].Select(long.Parse))
        .Fold((t, d) => t.Zip(d).Select(x => new Race(x.First, x.Second)))
        .Select(_ => _.NumWaysToWin()).Aggregate(1, (x, y) => x * y);

    public static object SolvePart2(ImmutableArray<string> input) => input
        .Select(_ => _[9..].Replace(" ", ""))
        .Select(long.Parse)
        .Fold((t, d) => new Race(t, d))
        .NumWaysToWin();
}