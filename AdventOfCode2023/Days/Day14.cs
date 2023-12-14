namespace AdventOfCode2023.Solutions;

public record Day14 : Day<Day14>, IDay<Day14>
{
    public static int DayNumber => 14;

    public static object SolvePart1(ImmutableArray<string> input) => RollVertical([.. input.Select(_ => _.ToArray())], true).Sum(_ => input.Length - _.y);

    public static object SolvePart2(ImmutableArray<string> input) => Solve([..input.Select(_ => _.ToArray())], (int)1E9);

    private static int Solve(char[][] grid, int cycles)
    {
        string Hash(IEnumerable<(int x, int y)> positions) => positions
            .Select(_ => _.x * grid.Length + _.y)
            .Order()
            .ToDelimitedString("-");

        int load = 0;
        Dictionary<string, (int cycle, int load)> visited = [];
        for (var i = 0; i < cycles; i++)
        {
            RollVertical(grid, true);
            RollHorizontal(grid, true);
            RollVertical(grid, false);
            var positions = RollHorizontal(grid, false);

            var hash = Hash(positions);
            if (visited.TryGetValue(hash, out var previousVisit))
            {
                var loopStart = previousVisit.cycle;
                var loopLength = i - loopStart;
                var targetCycle = loopStart - 1 + ((cycles - loopStart)  % loopLength);
                return visited.Values.First(_ => _.cycle == targetCycle).load;
            }

            load = positions.Sum(_ => grid.Length - _.y);
            visited.Add(hash, (i, load));
        }

        return load;
    }

    public static List<(int x, int y)> RollHorizontal(char[][] grid, bool isWest)
    {
        List<(int x, int y)> result = [];
        for (var j = 0; j < grid.Length; j++)
        {
            List<int> restPositions = [];
            if (isWest)
            {
                var lastStatic = -1;
                for (var i = 0; i < grid[j].Length; i++)
                {
                    if (grid[j][i] is '#') lastStatic = i;
                    if (grid[j][i] is 'O')
                    {
                        var restPosition = Math.Max(lastStatic, restPositions.LastOrDefault(-1)) + 1;
                        grid[j][i] = '.';
                        grid[j][restPosition] = 'O';
                        restPositions.Add(restPosition);
                        result.Add((restPosition, j));
                    }
                }
            }
            else
            {
                var lastStatic = grid[j].Length;
                for (var i = grid[j].Length - 1; i >= 0; i--)
                {
                    if (grid[j][i] is '#') lastStatic = i;
                    if (grid[j][i] is 'O')
                    {
                        var restPosition = Math.Min(lastStatic, restPositions.LastOrDefault(grid[j].Length)) - 1;
                        grid[j][i] = '.';
                        grid[j][restPosition] = 'O';
                        restPositions.Add(restPosition);
                        result.Add((restPosition, j));
                    }
                }
            }
        }
        return result;
    }

    public static List<(int x, int y)> RollVertical(char[][] grid, bool isNorth)
    {
        List<(int x, int y)> result = [];
        for (var i = 0; i < grid[0].Length; i++)
        {
            List<int> restPositions = [];
            if (isNorth)
            {
                var lastStatic = -1;
                for (var j = 0; j < grid.Length; j++)
                {
                    if (grid[j][i] is '#') lastStatic = j;
                    if (grid[j][i] is 'O')
                    {
                        var restPosition = Math.Max(lastStatic, restPositions.LastOrDefault(-1)) + 1;
                        grid[j][i] = '.';
                        grid[restPosition][i] = 'O';
                        restPositions.Add(restPosition);
                        result.Add((i, restPosition));
                    }
                }
            }
            else
            {
                var lastStatic = grid[0].Length;
                for (var j = grid[0].Length - 1; j >= 0; j--)
                {
                    if (grid[j][i] is '#') lastStatic = j;
                    if (grid[j][i] is 'O')
                    {
                        var restPosition = Math.Min(lastStatic, restPositions.LastOrDefault(grid[0].Length)) - 1;
                        grid[j][i] = '.';
                        grid[restPosition][i] = 'O';
                        restPositions.Add(restPosition);
                        result.Add((i, restPosition));
                    }
                }
            }
        }
        return result;
    }
}