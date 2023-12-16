using System.Diagnostics;
using static AdventOfCode2023.Solutions.Day16.Direction;

namespace AdventOfCode2023.Solutions;

public record Day16 : Day<Day16>, IDay<Day16>
{
    public static int DayNumber => 16;

    public enum Direction { Up, Down, Left, Right }

    public static object SolvePart1(ImmutableArray<string> input) => Energise([.. input], (-1, 0, Right));

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var grid = input.ToArray();

        var horizontalSources = Enumerable.Range(0, grid.Length).SelectMany(_ => new[] { (-1, _, Right), (grid[0].Length, _, Left) });
        var verticalSources = Enumerable.Range(0, grid[0].Length).SelectMany(_ => new[] { (_, -1, Down), (_, grid.Length, Up) });

        return horizontalSources.Concat(verticalSources).Max(_ => Energise(grid, _));
    }

    private static int Energise(string[] grid, (int x, int y, Direction direction) source)
    {
        var visited = new HashSet<(int x, int y)>();
        var visitedDirectional = new HashSet<(int x, int y, Direction direction)>();
        void Beam((int x, int y) pos, Direction direction)
        {
            if (visitedDirectional.Contains((pos.x, pos.y, direction))) return;
            visitedDirectional.Add((pos.x, pos.y, direction));
            visited.Add((pos.x, pos.y));

            pos = direction switch {
                Up => (pos.x, pos.y - 1),
                Down => (pos.x, pos.y + 1),
                Left => (pos.x - 1, pos.y),
                Right => (pos.x + 1, pos.y),
                _ => throw new UnreachableException()
            };

            if (pos.x < 0 || pos.y < 0 || pos.x >= grid[0].Length || pos.y >= grid.Length) return;

            var tile = grid[pos.y][pos.x];
            switch (tile)
            {
                case '|' when direction is Left or Right:
                    Beam(pos, Up);
                    Beam(pos, Down);
                    return;
                case '-' when direction is Up or Down:
                    Beam(pos, Left);
                    Beam(pos, Right);
                    return;
                case '\\':
                    direction = direction switch
                    {
                        Up => Left,
                        Left => Up,
                        Down => Right,
                        Right => Down,
                        _ => throw new UnreachableException()
                    };
                    break;
                case '/':
                    direction = direction switch
                    {
                        Up => Right,
                        Right => Up,
                        Down => Left,
                        Left => Down,
                        _ => throw new UnreachableException()
                    };
                    break;
            }
            Beam(pos, direction);
        }

        Beam((source.x, source.y), source.direction);
        return visited.Count - 1;
    }
}