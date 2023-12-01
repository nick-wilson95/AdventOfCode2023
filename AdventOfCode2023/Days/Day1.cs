namespace AdventOfCode2023.Solutions;

public record Day1 : Day<Day1>, IDay<Day1>
{
    public static int DayNumber => 1;

    private static readonly Dictionary<string, int> Digits = new()
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9 
    };

    private static int? ToInt(char c) => int.TryParse(c.ToString(), out var result) ? result : null;

    public static object SolvePart1(ImmutableArray<string> input) =>
        10 * input.Sum(_ => _.Select(ToInt).First(_ => _ is not null))!
        + input.Sum(_ => _.Reverse().Select(ToInt).First(_ => _ is not null))!;

    public static object SolvePart2(ImmutableArray<string> input)
    {
        static bool TryGetDigit(string line, int start, int end, out int digit)
        {
            digit = 0;
            return start >= 0
                && end < line.Length
                && Digits.TryGetValue(line[start..(end + 1)], out digit);
        }

        static int GetCalibration(string line)
        {
            var digit1 = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (int.TryParse(line[i].ToString(), out digit1)) break;
                if (TryGetDigit(line, i - 2, i, out digit1)) break;
                if (TryGetDigit(line, i - 3, i, out digit1)) break;
                if (TryGetDigit(line, i - 4, i, out digit1)) break;
            }

            var digit2 = 0;
            for (var i = line.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(line[i].ToString(), out digit2)) break;
                if (TryGetDigit(line, i, i + 2, out digit2)) break;
                if (TryGetDigit(line, i, i + 3, out digit2)) break;
                if (TryGetDigit(line, i, i + 4, out digit2)) break;
            }

            return 10 * digit1 + digit2;
        }

        return input.Sum(GetCalibration);
    }
}
