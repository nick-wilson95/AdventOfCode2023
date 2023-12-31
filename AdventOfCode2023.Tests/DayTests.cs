using AdventOfCode2023.Solutions;
using Xunit;

namespace AdventOfCode2023.Tests;

public class DayTests
{
    [Fact] public void Day0() => TestDay<Day0>(new("Test input", "Solved"));

    [Fact] public void Day1() => TestDay<Day1>(new(52974, 53340));

    [Fact] public void Day2() => TestDay<Day2>(new(2377, 71220));

    [Fact] public void Day3() => TestDay<Day3>(new(550064, 85010461));

    [Fact] public void Day4() => TestDay<Day4>(new(22193, 5625994));

    [Fact] public void Day5() => TestDay<Day5>(new((long)165788812, (long)1928058));

    [Fact] public void Day6() => TestDay<Day6>(new(771628, 27363861));

    [Fact] public void Day7() => TestDay<Day7>(new(250347426, 251224870));

    [Fact] public void Day8() => TestDay<Day8>(new(14429, 10921547990923));

    [Fact] public void Day9() => TestDay<Day9>(new(2098530125, 1016));

    [Fact] public void Day10() => TestDay<Day10>(new(7173, 291));

    [Fact] public void Day11() => TestDay<Day11>(new((long)9734203, 568914596391));

    [Fact] public void Day12() => TestDay<Day12>(new((long)7007, 3476169006222));

    [Fact] public void Day13() => TestDay<Day13>(new(36015, 35335));

    [Fact] public void Day14() => TestDay<Day14>(new(110407, 87273));

    [Fact] public void Day15() => TestDay<Day15>(new(516804, 231844));

    [Fact] public void Day16() => TestDay<Day16>(new(7074, 7530));

    [Fact] public void Day17() => TestDay<Day17>(new(1039, 1201));

    [Fact] public void Day18() => TestDay<Day18>(new((long)92758, 62762509300678));

    [Fact] public void Day19() => TestDay<Day19>(new(353553, 124615747767410));

    [Fact] public void Day20() => TestDay<Day20>(new(873301506, 241823802412393));

    [Fact] public void Day21() => TestDay<Day21>(new(3578, 594115391548176));

    [Fact] public void Day22() => TestDay<Day22>(new(426, 61920));

    [Fact] public void Day23() => TestDay<Day23>(new(2238, 6398));

    [Fact] public void Day24() => TestDay<Day24>(new(18184, 557789988450159d));

    [Fact] public void Day25() => TestDay<Day25>(new(562978, "TBC"));

    private static void TestDay<T>(Solution expected) where T : Day<T>, IDay<T>
        => Assert.Equal(expected, Day<T>.Solve());
}