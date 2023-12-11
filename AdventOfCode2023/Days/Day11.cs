using MoreLinq;
using Coord = (long Row, long Col);

namespace AdventOfCode2023.Solutions;

public record Day11 : Day<Day11>, IDay<Day11>
{
    public static int DayNumber => 11;

    public static object Solve(ImmutableArray<string> input, long interGalacticDistance)
    {
        var locations = input
            .Index()
            .SelectMany(x => x.Value
                .Index()
                .Where(y => y.Value is '#')
                .Select(y => new Coord(x.Key, y.Key)));

        Dictionary<long, long> DistsFromFirst(IEnumerable<long> values) => values
            .Order()
            .Pairwise((a, b) => (a, b))
            .Where(_ => _.b > _.a)
            .Select(_ => (value: _.b, dist: 1 + ((_.b - _.a - 1) * interGalacticDistance)))
            .Scan((a, b) => (b.value, a.dist + b.dist))
            .Prepend((value: values.Min(), dist: 0))
            .ToDictionary(_ => _.value, _ => _.dist);

        var vDists = DistsFromFirst(locations.Select(_ => _.Row));
        var hDists = DistsFromFirst(locations.Select(_ => _.Col));

        long GetDistance(Coord l1, Coord l2) => Math.Abs(vDists[l1.Row] - vDists[l2.Row]) + Math.Abs(hDists[l1.Col] - hDists[l2.Col]);

        return locations.Cartesian(locations, GetDistance).Sum() / 2;
    }

    public static object SolvePart1(ImmutableArray<string> input) => Solve(input, 2);

    public static object SolvePart2(ImmutableArray<string> input) => Solve(input, 1000000);
}