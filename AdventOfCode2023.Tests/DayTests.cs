using AdventOfCode2023.Solutions;
using Xunit;

namespace AdventOfCode2023.Tests;

public class DayTests
{
    [Fact] public void Day0() => TestDay<Day0>(new("Test input", "Solved"));

    [Fact] public void Day1() => TestDay<Day1>(new(52974, 53340));

    private static void TestDay<T>(Solution expected) where T : Day<T>, IDay<T>
        => Assert.Equal(expected, Day<T>.Solve());
}