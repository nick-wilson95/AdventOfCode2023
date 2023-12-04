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

    private static void TestDay<T>(Solution expected) where T : Day<T>, IDay<T>
        => Assert.Equal(expected, Day<T>.Solve());
}