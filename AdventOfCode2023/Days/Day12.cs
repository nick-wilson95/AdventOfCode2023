using MoreLinq;

namespace AdventOfCode2023.Solutions;

public record Day12 : Day<Day12>, IDay<Day12>
{
    public static int DayNumber => 12;

    private static long NumSolutions(string springs, int[] groups)
    {
        int[] CharLocations(char c) => springs.Index().Where(_ => _.Value == c).Select(_ => _.Key).Order().Prepend(-1).ToArray();

        var dotLocations = CharLocations('.');
        var hashLocations = CharLocations('#');
        var minSpace = groups.Index().Select(_ => groups[_.Key..].Sum() + groups.Length - _.Key - 1).ToArray();

        var cache = new Dictionary<(int, int), long>();
        long Solve(int springIndex, int groupIndex)
        {
            if (cache.TryGetValue((springIndex, groupIndex), out var cached)) return cached;

            if (groupIndex == groups.Length) return hashLocations[^1] >= springIndex ? 0 : 1;

            var groupSize = groups[groupIndex];
            if (groupIndex == groups.Length - 1 && springs.Length - springIndex == groupSize) return dotLocations[^1] >= springIndex ? 0 : 1;
            if (springs.Length - springIndex < minSpace[groupIndex]) return 0;

            long result = 0;
            if (springs[springIndex] is not '#')
            {
                result += Solve(springIndex + 1, groupIndex);
            }
            if (!springs[springIndex..(springIndex + groupSize)].Contains('.') && springs[springIndex + groupSize] is not '#')
            {
                result += Solve(springIndex + groupSize + 1, groupIndex + 1);
            }

            cache.Add((springIndex, groupIndex), result);
            return result;
        }

        return Solve(0, 0);
    }

    public static long Solve(ImmutableArray<string> input, int repeats) =>
        input.Sum(line => line.Split(' ').Fold((a, b) =>
            NumSolutions(
                Enumerable.Repeat(a, repeats).ToDelimitedString("?"),
                b.Split(',').Select(int.Parse).Repeat(repeats).ToArray())));

    public static object SolvePart1(ImmutableArray<string> input) => Solve(input, 1);

    public static object SolvePart2(ImmutableArray<string> input) => Solve(input, 5);
}