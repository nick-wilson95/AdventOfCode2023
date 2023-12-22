namespace AdventOfCode2023.Solutions;

public record Day22 : Day<Day22>, IDay<Day22>
{
    public static int DayNumber => 22;

    private record Position(int X, int Y, int Z)
    {
        public static Position Parse(string str) => str
            .Split(',')
            .Select(int.Parse)
            .Fold((x, y, z) => new Position(x, y, z));
    }

    // Brick first position coords are always <= last position coords
    private record Brick(Position A, Position B)
    {
        public static Brick Parse(string str) => str
            .Split('~')
            .Fold((a, b) => new Brick(A: Position.Parse(a), B: Position.Parse(b)));

        public Brick MoveBottom(int newBottom) => new(
            A with { Z = newBottom },
            B with { Z = newBottom + B.Z - A.Z }
        );

        public bool IsUnder(Brick brick)
        {
            if (B.Z >= brick.A.Z) return false;

            return (B.X >= brick.A.X)
                && (A.X <= brick.B.X)
                && (B.Y >= brick.A.Y)
                && (A.Y <= brick.B.Y);
        }
    }

    private static (Dictionary<int, List<int>> Supporting, Dictionary<int, List<int>> SupportedBy) GetRestStructure(ImmutableArray<string> input)
    {
        var bricks = input
            .Select(Brick.Parse)
            .OrderBy(_ => _.B.Z)
            .ToList();

        for (var i = 0; i < bricks.Count; i++)
        {
            var brick = bricks[i];
            int newBottom = 1;
            for (var j = i - 1; j >= 0; j--)
            {
                if (bricks[j].IsUnder(brick))
                {
                    newBottom = bricks[j].B.Z + 1;
                    break;
                }
            }

            if (newBottom == brick.A.Z) continue;

            bricks.RemoveAt(i);
            var newBrick = brick.MoveBottom(newBottom);

            if (newBrick.B.Z > bricks[^1].B.Z)
            {
                bricks.Add(newBrick);
            }
            else
            {
                var newIndex = bricks.Index().SkipWhile(_ => _.Value.B.Z < newBrick.B.Z).First().Key;
                bricks.Insert(newIndex, newBrick);
            }
        }

        var supporting = bricks.Index().ToDictionary(_ => _.Key, _ => new List<int>());
        var supportedBy = bricks.Index().ToDictionary(_ => _.Key, _ => new List<int>());
        for (var i = 0; i < bricks.Count; i++)
        {
            var brick = bricks[i];
            for (var j = 0; j < bricks.Count; j++)
            {
                var other = bricks[j];
                if (other.A.Z == brick.B.Z + 1 && brick.IsUnder(other))
                {
                    supporting[i].Add(j);
                    supportedBy[j].Add(i);
                }
            }
        }

        return (supporting, supportedBy);
    }

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var (Supporting, SupportedBy) = GetRestStructure(input);
        return Supporting.Count(i => i.Value.All(_ => SupportedBy[_].Count > 1));
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var (Supporting, SupportedBy) = GetRestStructure(input);

        var initiallyAtBottom = SupportedBy.Count(_ => _.Value.Count is 0);

        var total = 0;
        foreach (var disintegrateAt in Supporting.Keys)
        {
            var supporting = Supporting.ToDictionary(_ => _.Key, _ => _.Value.ToList());
            var supportedBy = SupportedBy.ToDictionary(_ => _.Key, _ => _.Value.ToList());

            var indices = new Queue<int>(new[] { disintegrateAt });
            while (indices.TryDequeue(out var index))
            {
                foreach (var i in supporting[index].ToArray())
                {
                    supporting[index].Remove(i);
                    supportedBy[i].Remove(index);
                    if (supportedBy[i].Count is 0) indices.Enqueue(i);
                }
            }
            total += supportedBy.Values.Count(_ => _.Count is 0) - initiallyAtBottom;
        }

        return total;
    }
}