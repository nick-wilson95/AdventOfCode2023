using System.Diagnostics;
using static AdventOfCode2023.Solutions.Day17.Direction;

namespace AdventOfCode2023.Solutions;

public record Day17 : Day<Day17>, IDay<Day17>
{
    public static int DayNumber => 17;

    public enum Direction { Up, Down, Left, Right };

    public static object SolvePart1(ImmutableArray<string> input) => Solve(input, 1, 3);

    public static object SolvePart2(ImmutableArray<string> input) => Solve(input, 4, 10);

    public static int Solve(ImmutableArray<string> input, int minMoves, int maxMoves)
    {
        var grid = input.Select(_ => _.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

        IEnumerable<(int moves, Direction direction, int x, int y)> GetAdjacent(int moves, Direction direction, int x, int y)
        {
            if (moves >= minMoves)
            {
                if (direction is not (Up or Down) && y > 0) yield return (1, Up, x, y - 1);
                if (direction is not (Left or Right) && x > 0) yield return (1, Left, x - 1, y);
                if (direction is not (Up or Down) && y < grid.Length - 1) yield return (1, Down, x, y + 1);
                if (direction is not (Left or Right) && x < grid[0].Length - 1) yield return (1, Right, x + 1, y);
            }
            if (moves < maxMoves)
            {
                if (direction is Up && y > 0) yield return (moves + 1, Up, x, y - 1);
                if (direction is Left && x > 0) yield return (moves + 1, Left, x - 1, y);
                if (direction is Down && y < grid.Length - 1) yield return (moves + 1, Down, x, y + 1);
                if (direction is Right && x < grid[0].Length - 1) yield return (moves + 1, Right, x + 1, y);
            }
        }

        var start = (minMoves, (Direction)(-1), 0, 0);
        PriorityQueue<(int moves, Direction direction, int x, int y), int> unvisited = new([(start, 0)]);
        Dictionary<(int moves, Direction direction, int x, int y), int> distances = new() { [start] = 0 };

        while (true)
        {
            var current = unvisited.Dequeue();
            var currentDistance = distances[current];
            if (current.x == input[0].Length - 1 && current.y == input.Length - 1 && current.moves >= minMoves)
            {
                return currentDistance;
            }

            GetAdjacent(current.moves, current.direction, current.x, current.y)
                .ToList()
                .Where(_ => !distances.ContainsKey(_))
                .Select(_ => (node: _, distance: currentDistance + grid[_.y][_.x]))
                .OrderBy(_ => _.distance)
                .ForEach(_ =>
                {
                    distances.Add(_.node, _.distance);
                    unvisited.Enqueue(_.node, _.distance);
                });
        }

        throw new UnreachableException();
    }
}