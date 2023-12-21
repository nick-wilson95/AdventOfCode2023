
namespace AdventOfCode2023.Solutions;

public record Day21 : Day<Day21>, IDay<Day21>
{
    public static int DayNumber => 21;

    private record Position(int X, int Y)
    {
        public Position[] Adjacent => [new(X + 1, Y), new(X - 1, Y), new(X, Y + 1), new(X, Y-1)];
    }

    // Assumptions:
    // Start is at (65, 65)
    public static object SolvePart1(ImmutableArray<string> input) => FillFrom(input, 65, 65, 64);

    // Assumptions:
    // Grid width and height is 131
    // Start is in the middle
    // Edge locations are reached from the centre in an expanding diamond pattern
    // There is a clear line between adjacent grid centres
    // There is clear space around the edge of the grid
    public static object SolvePart2(ImmutableArray<string> input)
    {
        long numSteps = 26501365;

        var saturatedEven = FillFrom(input, 65, 65, 1000);
        var saturatedOdd = FillFrom(input, 65, 65, 1001);

        var size = 131;
        var toFirstEdge = size / 2;

        var edgeSaturated = numSteps / size - 1;
        var edgeOddSaturated = (edgeSaturated + 1) / 2;
        var edgeEvenSaturated = edgeSaturated / 2;
        var edgeStepRemainder = numSteps - edgeSaturated * size - toFirstEdge - 1;

        var evenCornerSaturated = (long)Math.Pow(edgeEvenSaturated, 2);
        var oddCornerSaturated = (long)Math.Pow(edgeOddSaturated, 2) - edgeOddSaturated;

        var evenCornerRemainder = (numSteps - (toFirstEdge + 1) * 2) % (2 * size);
        var evenCornerCount = edgeEvenSaturated * 2 + 1;

        var oddCornerRemainder = (numSteps - (toFirstEdge + 1) * 2 - size) % (2 * size);
        var oddCornerCount = edgeOddSaturated * 2;

        var result = saturatedOdd
            + 4 * edgeOddSaturated * saturatedEven
            + 4 * edgeEvenSaturated * saturatedOdd
            + FillFrom(input, 0, 65, edgeStepRemainder)
            + FillFrom(input, 65, 0, edgeStepRemainder)
            + FillFrom(input, 130, 65, edgeStepRemainder)
            + FillFrom(input, 65, 130, edgeStepRemainder)
            + 4 * evenCornerSaturated * saturatedOdd
            + 4 * oddCornerSaturated * saturatedEven
            + evenCornerCount * FillFrom(input, 0, 0, evenCornerRemainder) + oddCornerCount * FillFrom(input, 0, 0, oddCornerRemainder)
            + evenCornerCount * FillFrom(input, 0, 130, evenCornerRemainder) + oddCornerCount * FillFrom(input, 0, 130, oddCornerRemainder)
            + evenCornerCount * FillFrom(input, 130, 0, evenCornerRemainder) + oddCornerCount * FillFrom(input, 130, 0, oddCornerRemainder)
            + evenCornerCount * FillFrom(input, 130, 130, evenCornerRemainder) + oddCornerCount * FillFrom(input, 130, 130, oddCornerRemainder);

        return result;
    }


    static int FillFrom(ImmutableArray<string> input, int startX, int startY, long steps)
    {
        Position start = new(startX, startY);
        var grid = new bool?[input[0].Length, input.Length];
        for (var j = 0; j < input.Length; j++)
        {
            for (var i = 0; i < input[0].Length; i++)
            {
                if (input[j][i] is '#') grid[i, j] = false;
            }
        }

        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        bool IsGarden(Position pos) =>
            pos.X >= 0 && pos.X < width && pos.Y >= 0 && pos.Y < height
            && grid![pos.X, pos.Y] is not false;

        HashSet<Position> checkFrom = [start];
        HashSet<Position> visitedOdd = [];
        HashSet<Position> visitedEven = [start];
        for (var i = 0; true; i++)
        {
            if (checkFrom.Count == 0)
            {
                break;
            }

            if (steps == i * 2) return visitedEven.Count;
            if (steps == i * 2 - 1) return visitedOdd.Count;

            HashSet<Position> newlyVisited = [];
            foreach (var pos in checkFrom)
            {
                var adjacent = pos.Adjacent.Where(IsGarden).Where(_ => grid[_.X, _.Y] is null);
                foreach (var p in adjacent) visitedOdd.Add(p);

                var twoAdjacent = adjacent.SelectMany(_ => _.Adjacent)
                    .Where(IsGarden).Where(_ => grid[_.X, _.Y] is null)
                    .Distinct();

                foreach (var p in twoAdjacent)
                {
                    grid[p.X, p.Y] = true;
                    visitedEven.Add(p);
                    newlyVisited.Add(p);
                }
            }

            checkFrom = newlyVisited;
        }

        return steps % 2 == 0
            ? visitedEven.Count
            : visitedOdd.Count;
    }
}