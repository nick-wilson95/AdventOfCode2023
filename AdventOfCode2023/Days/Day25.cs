using System.Collections.Frozen;

namespace AdventOfCode2023.Solutions;

public record Day25 : Day<Day25>, IDay<Day25>
{
    public static int DayNumber => 25;

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var connectionsLiquid = new Dictionary<string, List<string>>();

        foreach (var item in input)
        {
            var parts = item.Split(": ").ToArray();
            var from = parts[0];
            var tos = parts[1].Split(" ");

            if (!connectionsLiquid.ContainsKey(from)) connectionsLiquid.Add(from, []);

            foreach (var to in tos)
            {
                if (!connectionsLiquid.ContainsKey(to)) connectionsLiquid.Add(to, []);
                connectionsLiquid[to].Add(from);
                connectionsLiquid[from].Add(to);
            }
        }

        var connections = connectionsLiquid.ToFrozenDictionary();
        var nodes = connections.Keys.ToArray();

        HashSet<string> groupA = [ nodes[0] ];
        bool Has4DistinctPaths(string from)
        {
            HashSet<(string, string)> used = [];
            for (var i = 0; i < 4; i++)
            {
                Queue<string[]> check = new([[from]]);
                HashSet<string> visited = [from];

                var success = false;
                while (check.Count > 0)
                {
                    var path = check.Dequeue();
                    var pos = path[^1];

                    if (groupA.Contains(pos))
                    {
                        path.Pairwise((a, b) => (a, b)).ForEach(_ => used.Add(_));
                        success = true;
                        break;
                    }

                    visited.Add(pos);

                    foreach (var item in connections[pos])
                    {
                        if (visited.Contains(item) || used.Contains((pos, item)) || used.Contains((item, pos))) continue;
                        check.Enqueue([..path, item]);
                    }
                }
                if (success) continue;

                return false;
            }

            return true;
        }

        for (var i = 1; i < nodes.Length; i++)
        {
            if (Has4DistinctPaths(nodes[i]))
            {
                groupA.Add(nodes[i]);
            }
        }

        return groupA.Count * (nodes.Length - groupA.Count);
    }
}