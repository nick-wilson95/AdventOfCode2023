using System.Diagnostics;
using System.Globalization;

namespace AdventOfCode2023.Solutions;

public record Day18 : Day<Day18>, IDay<Day18>
{
    public static int DayNumber => 18;

    public static object SolvePart1(ImmutableArray<string> input) => Solve(input
        .Select(_ => _.Split(' ').Fold((a, b, c) => (
            Direction: a.Single(),
            Distance: int.Parse(b)
        ))));

    public static object SolvePart2(ImmutableArray<string> input) => Solve(input
        .Select(_ => _.Split(' ')[2][2..^1])
        .Select(_ => (
            Direction: _[^1] switch { '0' => 'R', '1' => 'D', '2' => 'L', '3' => 'U', _ => throw new UnreachableException() },
            Distance: int.Parse(_[..^1], NumberStyles.HexNumber)
        )));

    public static long Solve(IEnumerable<(char Direction, int Distance)> instructions)
    {
        List<(int x, int y)> vertices = [(0, 0)];
        foreach (var (direction, distance) in instructions)
        {
            var (x, y) = vertices[^1];
            var next = direction switch
            {
                'U' => (x, y + distance),
                'D' => (x, y - distance),
                'L' => (x - distance, y),
                'R' => (x + distance, y),
                _ => throw new UnreachableException()
            };
            vertices.Add(next);
        }

        var edges = vertices.Pairwise((a, b) => (a, b));
        var verticals = edges.Where(_ => _.a.y != _.b.y)
            .Select(_ => (_.a.x, minY: Math.Min(_.a.y, _.b.y), maxY: Math.Max(_.a.y, _.b.y)))
            .OrderBy(_ => _.x);
        var horizontals = edges.Where(_ => _.a.x != _.b.x)
            .Select(_ => (_.a.y, minX: Math.Min(_.a.x, _.b.x), maxX: Math.Max(_.a.x, _.b.x)))
            .OrderBy(_ => _.y);

        var orderedY = vertices.Select(_ => _.y).Distinct().Order().ToArray();

        long volume = 0;
        for (var i = 0; i < orderedY.Length - 1; i++)
        {
            var startY = orderedY[i];
            var endY = orderedY[i + 1];

            var topHorizontals = horizontals.Where(_ => _.y == startY);
            var bottomHorizontals = horizontals.Where(_ => _.y == endY);

            var totalY = endY - startY;
            var totalX = 0;
            var extraHorizontalArea = 0;

            int? startX = null;
            int? topRightEnd = null;
            foreach (var x in verticals.Where(_ => _.minY <= startY && _.maxY >= endY).Select(_ => _.x))
            {
                if (startX is null)
                {
                    startX = x;
                    continue;
                }
                if (topHorizontals.TryGetSingle(_ => _.maxX == startX, out var topLeft))
                {
                    extraHorizontalArea += topLeft.maxX == topRightEnd
                        ? -1
                        : topLeft.maxX - topLeft.minX;

                    topRightEnd = null;
                }
                if (topHorizontals.TryGetSingle(_ => _.minX == x, out var topRight))
                {
                    extraHorizontalArea += topRight.maxX - topRight.minX;
                    topRightEnd = topRight.maxX;
                }
                if (bottomHorizontals.TryGetSingle(_ => _.minX == startX && _.maxX == x, out var bottom))
                {
                    extraHorizontalArea += bottom.maxX - bottom.minX + 1;
                }

                totalX += x - startX.Value + 1;
                startX = null;
            }
            volume += (long)totalX * totalY + extraHorizontalArea;
        }

        return volume;
    }
}