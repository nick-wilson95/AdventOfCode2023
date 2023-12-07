using MoreLinq;

namespace AdventOfCode2023.Solutions;

public record Day7 : Day<Day7>, IDay<Day7>
{
    public static int DayNumber => 7;

    public record Player(int[] Hand, int Bid);

    public static int CardValue(char c, bool jokers) => c switch
    {
        'A' => 13,
        'K' => 12,
        'Q' => 11,
        'J' => jokers ? 0 : 10,
        'T' => 9,
        _ => int.Parse(c.ToString()) - 1
    };

    public static int HandTypeScore(int[] hand)
    {
        var jokerCount = hand.Count(_ => _ == 0);

        var counts = hand
            .GroupBy(_ => _).Select(_ => _.Count())
            .OrderDescending()
            .ToArray();

        if (counts.Length is 1) return 7;

        return (counts[0], counts[1], jokerCount) switch
        {
            (5, _, _) or (4, _, 1) or (3, _, 2) or (3, 2, 3) or (_, _, 4) => 7, // 5 of a kind
            (4, _, _) or (3, _, 1) or (2, 2, 2) or (_, _, 3) => 6, // 4 of a kind
            (3, 2, _) or (3, _, 1) or (2, 2, 1) => 5, // Full house
            (3, _, _) or (2, _, 1) or (_, _, 2) => 4, // Three of a kind
            (2, 2, _) => 3, // Two pairs
            (2, _, _) or (_, _, 1) => 2, // One pair
            _ => 1
        };
    }

    public static int HandScore(Player player)
    {
        var handTypeScore = HandTypeScore(player.Hand);
        var valueScore = player.Hand.Index().Sum(_ => _.Value * (int)Math.Pow(14, 4 - _.Key));
        return valueScore + handTypeScore * (int)Math.Pow(14, 5);
    }

    public static int Solve(ImmutableArray<string> input, bool withJokers) => input
        .Select(_ => _.Split(' ')
        .Fold((a, b) => new Player(a.Select(_ => CardValue(_, withJokers)).ToArray(), int.Parse(b))))
        .OrderBy(HandScore)
        .Index()
        .Sum(_ => (_.Key + 1) * _.Value.Bid);

    public static object SolvePart1(ImmutableArray<string> input) => Solve(input, false);

    public static object SolvePart2(ImmutableArray<string> input) => Solve(input, true);
}