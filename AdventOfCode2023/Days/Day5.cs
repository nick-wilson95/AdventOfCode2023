using MapEntry = (long Destination, long Source, long Length);

namespace AdventOfCode2023.Solutions;

public record Day5 : Day<Day5>, IDay<Day5>
{
    public static int DayNumber => 5;

    private static IEnumerable<MapEntry> ReadMap(IEnumerable<string> lines) =>
        lines.Skip(1).Select(_ => _.Split(' ').Select(long.Parse).Fold((a, b, c) => new MapEntry(a, b, c)));

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var sections = input.Split(string.Empty);
        var seeds = sections.First().Single().Split(' ').Skip(1).Select(long.Parse);
        var maps = sections.Skip(1).Select(ReadMap);

        var result = long.MaxValue;
        foreach (var seed in seeds)
        {
            var subject = seed;
            foreach (var map in maps)
            {
                foreach (var (Destination, Source, Length) in map)
                {
                    if (Source <= subject && subject < Source + Length)
                    {
                        subject = Destination + subject - Source;
                        break;
                    }
                }
            }
            result = Math.Min(result, subject);
        }

        return result;
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var sections = input.Split(string.Empty);

        var seeds = sections.First().Single().Split(' ').Skip(1).Select(long.Parse)
            .Chunk(2)
            .Select(_ => Range.FromLength(_[0], _[1]));

        var maps = sections.Skip(1).Select(ReadMap).ToArray();

        long MinLocationNumber(Range range, int mapIndex)
        {
            if (mapIndex >= maps!.Length) return range.Start;

            List<Range> intersections = [];
            List<Range> shiftedIntersections = [];

            foreach (var (Destination, Source, Length) in maps[mapIndex])
            {
                var sourceRange = Range.FromLength(Source, Length);
                if (range.TryGetIntersection(sourceRange, out var intersection))
                {
                    intersections.Add(intersection);
                    shiftedIntersections.Add(intersection.Shift(Destination - Source));
                }
            }

            var gaps = intersections.OrderBy(_ => _.Start)
                .Prepend(new(range.Start - 1))
                .Append(new(range.End + 1))
                .Pairwise((r1, r2) => new Range(r1.End + 1, r2.Start - 1))
                .Where(_ => _.End >= _.Start);

            return gaps.Concat(shiftedIntersections).Min(_ => MinLocationNumber(_, mapIndex + 1));
        }

        return seeds.Select(_ => MinLocationNumber(_, 0)).Min();
    }

    private record Range(long Start, long End)
    {
        public Range(long singletonValue) : this(singletonValue, singletonValue) { }

        public Range Shift(long distance) => new(Start + distance, End + distance);

        public static Range FromLength(long start, long length) => new(start, start + length - 1);

        public bool TryGetIntersection(Range other, out Range intersection)
        {
            intersection = new(
                Math.Max(Start, other.Start),
                Math.Min(End, other.End)
            );

            return intersection.End >= intersection.Start;
        }
    }
}