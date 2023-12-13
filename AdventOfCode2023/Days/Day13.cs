namespace AdventOfCode2023.Solutions;

public record Day13 : Day<Day13>, IDay<Day13>
{
    public static int DayNumber => 13;

    public static object SolvePart1(ImmutableArray<string> input) => input.Split("").Sum(Summarise(0));

    public static object SolvePart2(ImmutableArray<string> input) => input.Split("").Sum(Summarise(1));

    private static Func<IEnumerable<string>, int> Summarise(int smudges) => (IEnumerable<string> grid) =>
        ReflectingColumns(grid, smudges).Sum()
        + ReflectingColumns(grid.Transpose().Select(_ => string.Concat(_)), smudges).Sum() * 100;

    private static IEnumerable<int> ReflectingColumns(IEnumerable<string> grid, int smudges)
    {
        var result = Enumerable.Range(1, grid.First().Length - 1)
            .ToDictionary(_ => _, _ => smudges);

        foreach (var row in grid)
        {
            foreach (var key in result.Keys.ToArray())
            {
                for (var i = 0; i < Math.Min(key, row.Length - key); i++)
                {
                    if (row[key + i] == row[key - i - 1]) continue;
                    if (result[key] == 0)
                    {
                        result.Remove(key);
                        break;
                    }
                    result[key]--;
                }
            }
        }

        return result.Where(_ => _.Value is 0)
            .Select(_ => _.Key);
    }
}