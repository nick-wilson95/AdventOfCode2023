using MoreLinq;
using System.Diagnostics;

namespace AdventOfCode2023.Solutions;

public record Day10 : Day<Day10>, IDay<Day10>
{
    public static int DayNumber => 10;

    private record Position(int X, int Y)
    {
        public Position Left => new(X - 1, Y);
        public Position Right => new(X + 1, Y);
        public Position Up => new(X, Y - 1);
        public Position Down => new(X, Y + 1);
    }

    static char? GetPipe(string[] landscape, Position pos) =>
        pos.X >= 0 && pos.X < landscape[0].Length && pos.Y >= 0 && pos.Y < landscape.Length
            ? landscape![pos.Y][pos.X]
            : null;

    private static Position GetStart(string[] landscape)
    {
        for (var j = 0; j < landscape.Length; j++)
        {
            for (var i = 0; i < landscape[0].Length; i++)
            {
                if (landscape[j][i] is 'S')
                {
                    return new(i, j);
                }
            }
        }
        throw new UnreachableException();
    }
    private static Position GetFirst(string[] landscape, Position start) =>
        GetPipe(landscape, start.Up) is '|' or '7' or 'F' ? start.Up
            : GetPipe(landscape, start.Right) is '-' or '7' or 'J' ? start.Right
            : start.Down;

    private static (Position next, int clockwiseQuarterTurns) GetNext(string[] landscape, Position pos, Position prev) =>
            GetPipe(landscape, pos) switch
            {
                '|' => prev == pos.Up ? (pos.Down, 0) : (pos.Up, 0),
                '-' => prev == pos.Left ? (pos.Right, 0) : (pos.Left, 0),
                'L' => prev == pos.Up ? (pos.Right, -1) : (pos.Up, 1),
                'J' => prev == pos.Up ? (pos.Left, 1) : (pos.Up, -1),
                '7' => prev == pos.Left ? (pos.Down, 1) : (pos.Left, -1),
                'F' => prev == pos.Right ? (pos.Down, -1) : (pos.Right, 1),
                var x => throw new UnreachableException($"pipe section is: {x}")
            };

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var landscape = input.ToArray();
        var start = GetStart(landscape);
        var first = GetFirst(landscape, start);

        List<Position> moves = [start, first];
        while (true)
        {
            if (moves.Last() == start) return moves.Count / 2;
            moves.Add(GetNext(landscape, moves[^1], moves[^2]).next);
        }
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var landscape = input.ToArray();
        var start = GetStart(landscape);
        var first = GetFirst(landscape, start);

        List<Position> moves = [start, first];
        var totalTurn = 0;
        while (true)
        {
            var (next, turn) = GetNext(landscape, moves[^1], moves[^2]);
            totalTurn += turn;
            moves.Add(next);
            if (moves.Last() == start) break;
        }

        var isClockwiseLoop = totalTurn > 0;

        return GetEnclosed(landscape, moves, isClockwiseLoop);
    }

    private static int GetEnclosed(string[] landscape, List<Position> path, bool isClockwiseLoop)
    {
        HashSet<Position> enclosed = [];
        var bounds = path.ToHashSet();
        for (var i = 1; i < path.Count; i++)
        {
            var (pos, prev) = (path[i], path[i - 1]);
            var pipe = GetPipe(landscape, pos);
            var sampleLeft = pipe switch
            {
                '|' => pos == prev.Up ^ isClockwiseLoop,
                'F' => pos == prev.Up ^ isClockwiseLoop,
                'L' => pos == prev.Left ^ isClockwiseLoop,
                _ => false
            };

            if (sampleLeft)
            {
                if (enclosed.Contains(pos.Left) || bounds.Contains(pos.Left)) continue;
                enclosed.UnionWith(GetConnected(landscape, bounds, pos.Left));
            }
        }

        return enclosed.Count;
    }

    private static HashSet<Position> GetConnected(string[] landscape, HashSet<Position> bounds, Position initial)
    {
        HashSet<Position> result = [];
        void CheckFrom(Position pos)
        {
            result.Add(pos);
            foreach (var item in new[] { pos.Up, pos.Right, pos.Down, pos.Left })
            {
                if (result.Contains(item) || bounds.Contains(item)) continue;
                CheckFrom(item);
            }
        }

        CheckFrom(initial);
        return result;
    }
}