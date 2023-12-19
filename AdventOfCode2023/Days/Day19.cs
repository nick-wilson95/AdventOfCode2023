using System.Diagnostics;

namespace AdventOfCode2023.Solutions;

public record Day19 : Day<Day19>, IDay<Day19>
{
    public static int DayNumber => 19;

    private static List<Bounds> GetAcceptedBounds(Dictionary<string, List<Operation>> workflows)
    {
        List<Bounds> accepted = [];
        void GetAcceptedBounds(Bounds bounds, string workflow)
        {
            var remainder = bounds;
            foreach (var operation in workflows[workflow])
            {
                (var intersect, remainder) = remainder.Split(operation);

                if (intersect is not null)
                {
                    if (operation.Result is "A") accepted.Add(intersect);
                    else if (operation.Result is not "R") GetAcceptedBounds(intersect, operation.Result);
                }

                if (remainder is null) break;
            }
        }

        GetAcceptedBounds(new Bounds(1, 4000), "in");
        return accepted;
    }

    private static (Dictionary<string, List<Operation>> Workflows, IEnumerable<Dictionary<char, int>> Parts) ParseInput(ImmutableArray<string> input) =>
        input.Split(string.Empty).Fold((s1, s2) => (
            s1.Select(_ => _.Trim('}').Split('{').Fold((a, b) => (a, b.Split(',').Select(Operation.Parse))))
                .ToDictionary(_ => _.a, _ => _.Item2.ToList()),
            s2.Select(_ => _[1..^1]
                .Split(',')
                .Select(_ => _.Split('='))
                .ToDictionary(_ => _[0][0], _ => int.Parse(_[1])))));

    private record Operation((char subject, char comparison, int threshold)? Condition, string Result)
    {
        public static Operation Parse(string str) => str.Contains(':')
            ? str.Split(':').Fold((a, b) => new Operation((a[0], a[1], int.Parse(a[2..])), b))
            : new(null, str);
    }

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var (workflows, parts) = ParseInput(input);

        var acceptedBounds = GetAcceptedBounds(workflows);

        return parts
            .Where(p => acceptedBounds.Any(_ => _.X.Contains(p['x']) && _.M.Contains(p['m']) && _.A.Contains(p['a']) && _.S.Contains(p['s'])))
            .SelectMany(_ => _.Values)
            .Sum();
    }

    public static object SolvePart2(ImmutableArray<string> input) =>
        GetAcceptedBounds(ParseInput(input).Workflows).Sum(_ => _.Volume);

    private record Bounds(Range X, Range M, Range A, Range S)
    {
        public Bounds(int min, int max) : this(new(min, max), new(min, max), new(min, max), new(min, max)) { }

        public long Volume => (long)X.Size * M.Size * A.Size * S.Size;

        public (Bounds? Intersect, Bounds? Complement) Split(Operation operation)
        {
            if (operation.Condition is null) return (this, null);
            var (subject, comparison, threshold) = operation.Condition.Value;

            var (intersect, complement) = (subject switch
            {
                'x' => X, 'm' => M, 'a' => A, 's' => S,
                _ => throw new UnreachableException()
            }).Split(comparison, threshold);

            (Bounds? Intersect, Bounds? Complement) Result(Func<Range, Bounds> setRange) =>
                (intersect is null ? null : setRange(intersect), complement is null ? null : setRange(complement));

            return subject switch
            {
                'x' => Result(_ => this with { X = _ }),
                'm' => Result(_ => this with { M = _ }),
                'a' => Result(_ => this with { A = _ }),
                's' => Result(_ => this with { S = _ }),
                _ => throw new UnreachableException()
            };
        }
    }

    private record Range(int Min, int Max)
    {
        public Range? NullIfEmpty => Max < Min ? null : this;

        public int Size => Max - Min + 1;

        public (Range? Intersect, Range? Complement) Split(char comparison, int threshold)
        {
            (Range Intersect, Range Complement) result = comparison is '<'
                ? (new(Min, threshold - 1), new(threshold, Max))
                : (new(threshold + 1, Max), new(Min, threshold));

            return (result.Intersect.NullIfEmpty, result.Complement.NullIfEmpty);
        }

        public bool Contains(int value) => Min <= value && value <= Max;
    }
}