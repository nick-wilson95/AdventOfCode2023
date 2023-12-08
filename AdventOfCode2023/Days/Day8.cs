using MoreLinq;
using System.Collections.Frozen;
using Node = (string Left, string Right);

namespace AdventOfCode2023.Solutions;

public record Day8 : Day<Day8>, IDay<Day8>
{
    public static int DayNumber => 8;

    public static (string Route, FrozenDictionary<string, (string Left, string Right)> Nodes) Parse(ImmutableArray<string> input) =>
        input.Split("").Fold((a, b) => (
            a.Single(),
            b.ToDictionary(_ => _[..3], _ => new Node(_[7..10], _[12..15])).ToFrozenDictionary()));

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var (route, nodes) = Parse(input);

        var position = "AAA";
        for (var i = 0; true; i++)
        {
            if (position is "ZZZ") return i;
            position = route[i % route.Length] is 'L'
                ? nodes[position].Left
                : nodes[position].Right;
        }
    }

    private record HitProfile(int[] Hits, int LoopFrom, int LoopLength);

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var (route, nodes) = Parse(input);

        HitProfile GetHitProfile(string initialPosition)
        {
            Dictionary<(string, int), int> hits = [];

            var position = initialPosition;
            for (int i = 0; true; i++)
            {
                if (position[2] is 'Z')
                {
                    var lastHit = hits.GetValueOrDefault((position, i % route.Length), -1);
                    if (lastHit != -1)
                    {
                        return new([.. hits.Values], lastHit, i - lastHit);
                    }
                    hits.Add((position, i % route.Length), i);
                }

                position = route[i % route.Length] is 'L'
                    ? nodes[position].Left
                    : nodes[position].Right;
            }
        }

        var hitProfiles = nodes.Keys.Where(_ => _[2] is 'A').Select(GetHitProfile).ToArray();

        // It turns out every path only goes through one 'Z' and loops regularly from 0
        // Therefore we can just take the LCM of the initial hit values and the rest of the hit profile calculation is academic...
        return hitProfiles.Select(x => (long)x.Hits[0]).Aggregate((long)1, Lcm, _ => _);
    }

    static long Gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long Lcm(long a, long b) => a * b / Gcf(a, b);
}