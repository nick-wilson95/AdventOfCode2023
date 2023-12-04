using MoreLinq;
using static System.StringSplitOptions;

namespace AdventOfCode2023.Solutions;

public record Day4 : Day<Day4>, IDay<Day4>
{
    public static int DayNumber => 4;

    private static Card ReadCard(string line) =>
        line.Split(':', '|')
            .Fold((_, a, b) => new Card(
                a.Split(' ', RemoveEmptyEntries).Select(int.Parse).ToArray(),
                b.Split(' ', RemoveEmptyEntries).Select(int.Parse).ToArray()
            ));

    public static object SolvePart1(ImmutableArray<string> input) => input
        .Select(ReadCard)
        .Sum(_ => (int)Math.Pow(2, _.NumMatches - 1));

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var cards = input.Select(ReadCard).ToArray();

        Dictionary<int, int> copies = [];
        for(var i = 0; i < cards.Length; i++)
        {
            copies[i] = copies.GetValueOrDefault(i) + 1;
            for (var j = 1; j <= cards[i].NumMatches; j++)
            {
                if (i + j > input.Length) break;
                copies[i + j] = copies.GetValueOrDefault(i + j) + copies[i];
            }
        }

        return copies.Values.Sum();
    }

    private record Card(int[] Winners, int[] Result)
    {
        public int NumMatches => Result.Count(_ => Winners.Contains(_));
    }
}