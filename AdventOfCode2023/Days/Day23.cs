namespace AdventOfCode2023.Solutions;

public record Day23 : Day<Day23>, IDay<Day23>
{
    public static int DayNumber => 23;

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var width = input[0].Length;
        var height = input.Length;

        var maxVisited = Enumerable.Range(0, width)
            .Cartesian(Enumerable.Range(0, height), (x, y) => (x, y))
            .ToDictionary(_ => _, _ => 0);

        var queue = new Queue<(int x, int y, (int, int)[] path)>([(1, 0, Array.Empty<(int, int)>())]);
        while (queue.Count > 0)
        {
            var (x, y, path) = queue.Dequeue();
            maxVisited[(x, y)] = path.Length;

            List<(int x, int y)> next = [];
            if (input[y][x - 1] is '.' or '<')
                next.Add((x - 1, y));

            if (input[y][x + 1] is '.' or '>')
                next.Add((x + 1, y));

            if (y > 0 && input[y - 1][x] is '.' or '^')
                next.Add((x, y - 1));

            if (y < height - 1 && input[y + 1][x] is '.' or 'v')
                next.Add((x, y + 1));

            foreach (var item in next)
            {
                if (maxVisited[item] <= path.Length && !path.Contains(item))
                {
                    queue.Enqueue((item.x, item.y, [.. path, item]));
                }
            }
        }

        return maxVisited[(width - 2, height - 1)];
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var width = input[0].Length;
        var height = input.Length;

        var start = (1, 0);
        var end = (width - 2, height - 1);

        List<(int x, int y)> nodes = [start, end];
        for (var i = 1; i < width - 1; i++)
        {
            for (var j = 1; j < height - 1; j++)
            {
                if (input[j][i] is '#') continue;

                var adjacent = new (int x, int y)[]
                {
                    (i - 1, j),
                    (i + 1, j),
                    (i, j - 1),
                    (i, j + 1),
                };

                if (adjacent.Sum(_ => input[_.y][_.x] is '#' ? 0 : 1) > 2)
                {
                    nodes.Add((i, j));
                }
            }
        }

        var edges = nodes.Select(_ => new List<(int i, int d)>()).ToArray();
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            var adj = new (int x, int y)[]
            {
                (node.x - 1, node.y),
                (node.x + 1, node.y),
                (node.x, node.y - 1),
                (node.x, node.y + 1),
            };

            foreach (var item in adj)
            {
                if (item.y <= 0 || item.y >= input.Length || input[item.y][item.x] is '#') continue;

                var prev = node;
                var p = item;
                for (var k = 1; true; k++)
                {
                    if (nodes.Contains(p))
                    {
                        if (!edges[i].Any(_ => nodes[_.i] == p))
                        {
                            var index = nodes.IndexOf(p);
                            edges[i].Add((index, k));
                            edges[index].Add((i, k));
                        }
                        break;
                    }

                    var adjacent = new (int x, int y)[]
                    {
                        (p.x - 1, p.y),
                        (p.x + 1, p.y),
                        (p.x, p.y - 1),
                        (p.x, p.y + 1),
                    };

                    foreach (var (x, y) in adjacent)
                    {
                        if (y >= 0 && y < input.Length
                            && input[y][x] is not '#'
                            && (x, y) != prev)
                        {
                            prev = p;
                            p = (x, y);
                            break;
                        }
                    }
                }
            }
        }

        List<int> lengths = [];
        void Traverse(int from, ulong visited, int distance)
        {
            if (from is 1)
            {
                lengths.Add(distance);
                return;
            }

            foreach (var (i, d) in edges[from])
            {
                if ((visited & ((ulong)1 << i)) != 0) continue;
                Traverse(i, visited | ((ulong)1 << i), distance + d);
            }
        }

        Traverse(0, 1, 0);
        return lengths.Max();
    }
}