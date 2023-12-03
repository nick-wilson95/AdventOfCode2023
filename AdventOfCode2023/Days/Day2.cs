namespace AdventOfCode2023.Solutions;

public record Day2 : Day<Day2>, IDay<Day2>
{
    public static int DayNumber => 2;

    public static object SolvePart1(ImmutableArray<string> input) => input.Select(Parse).Where(Valid).Sum(_ => _.Id);

    public static object SolvePart2(ImmutableArray<string> input) => input.Select(Parse).Sum(Power);

    public static Game Parse(string line)
    {
        static Dictionary<string, int> ParseRound(string roundString) => roundString
            .Split(',')
            .Select(_ => _.Trim().Split(' '))
            .ToDictionary(_ => _[1], _ => int.Parse(_[0]));

        var parts = line.Split(':');

        return new(
            Id: int.Parse(parts[0][5..]),
            Rounds: parts[1].Split(';').Select(ParseRound).ToArray()
        );
    }

    public static bool Valid(Game game) =>
        game.Rounds.Max(_ => _.ValueOrZero("red")) <= 12
        && game.Rounds.Max(_ => _.ValueOrZero("green")) <= 13
        && game.Rounds.Max(_ => _.ValueOrZero("blue")) <= 14;

    public static int Power(Game game) =>
        game.Rounds.Max(_ => _.ValueOrZero("red"))
        * game.Rounds.Max(_ => _.ValueOrZero("green"))
        * game.Rounds.Max(_ => _.ValueOrZero("blue"));

    public record Game(int Id, Dictionary<string, int>[] Rounds);
}