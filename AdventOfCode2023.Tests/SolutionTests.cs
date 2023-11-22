using AdventOfCode2023.Solutions;
using Xunit;

namespace AdventOfCode2023.Tests;

public class SolutionTests
{
    [Fact] public void Day0() => TestDay<Day0>(new("Test input", "Solved"));

    private static void TestDay<T>(Solution expected) where T : Day<T>, IDay<T>
        => Assert.Equal(expected, Day<T>.Solve());
}