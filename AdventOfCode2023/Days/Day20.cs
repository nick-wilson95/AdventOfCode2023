using System.Collections.Frozen;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode2023.Solutions;

public record Day20 : Day<Day20>, IDay<Day20>
{
    public static int DayNumber => 20;

    private abstract record Module(string Name, string[] Targets)
    {
        public abstract IEnumerable<(string from, string target, bool pulse)> Process(string from, bool isHigh);

        public abstract string GetState();
    }

    private record FlipFlop(string Name, string[] Targets) : Module(Name, Targets)
    {
        public override string GetState() => isOn ? "1" : "0";

        private bool isOn;
        public override IEnumerable<(string from, string target, bool pulse)> Process(string from, bool isHigh)
        {
            if (isHigh) return Array.Empty<(string, string, bool)>();

            isOn = !isOn;
            return Targets.Select(_ => (Name, _, isOn));
        }
    }

    private record Conjunction(string Name, string[] Targets) : Module(Name, Targets)
    {
        public override string GetState() => string.Join("", Memory.Values.Select(_ => _ ? "1" : "0"));

        public Dictionary<string, bool> Memory { get; } = [];

        public override IEnumerable<(string from, string target, bool pulse)> Process(string from, bool isHigh)
        {
            Memory[from] = isHigh;
            var sendHigh = Memory.Values.Any(_ => _ is false);
            return Targets.Select(_ => (Name, _, sendHigh));
        }
    }

    private record Broadcast(string Name, string[] Targets) : Module(Name, Targets)
    {
        public override string GetState() => "";

        public override IEnumerable<(string from, string target, bool pulse)> Process(string from, bool isHigh)
            => Targets.Select(_ => (Name, _, isHigh));
    }

    private static FrozenDictionary<string, Module> Read(ImmutableArray<string> input)
    {
        var modules = input.Select(_ => _.Split(" -> ").Fold<string, Module>((a, b) =>
        {
            var name = a[1..];
            var targets = b.Split(", ").ToArray();
            return a[0] switch
            {
                '%' => new FlipFlop(name, targets),
                '&' => new Conjunction(name, targets),
                'b' => new Broadcast("broadcaster", targets),
                _ => throw new UnreachableException()
            };
        })).ToDictionary(_ => _.Name, _ => _);

        foreach (var module in modules.Values)
        {
            foreach (var target in module.Targets)
            {
                if (modules.TryGetValue(target, out var x) && x is Conjunction conj)
                {
                    conj.Memory[module.Name] = false;
                }
            }
        }

        return modules.ToFrozenDictionary();
    }

    private static (int lowPulses, int highPulses) PressButton(FrozenDictionary<string, Module> modules)
    {
        var (low, high) = (0, 0);

        Queue<(string from, string target, bool isHigh)> pulseQueue = new(new[] { ("button", "broadcaster", false) });
        while (pulseQueue.Count > 0)
        {
            var (from, target, isHigh) = pulseQueue.Dequeue();

            if (isHigh) high++;
            else low++;

            if (modules.TryGetValue(target, out var module))
            {
                module.Process(from, isHigh).ForEach(pulseQueue.Enqueue);
            }
        }

        return (low, high);
    }

    public static object SolvePart1(ImmutableArray<string> input)
    {
        var modules = Read(input);

        return Enumerable.Range(0, 1000)
            .Select(_ => PressButton(modules))
            .Select(_ => new[] { _.lowPulses, _.highPulses })
            .Transpose()
            .Fold((a, b) => a.Sum() * b.Sum());
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var modules = Read(input);

        var dependencies = modules.ToDictionary(_ => _.Key, _ => new HashSet<string>());
        dependencies["rx"] = [];

        foreach (var module in modules.Values)
        {
            foreach (var target in module.Targets)
            {
                dependencies[target].Add(module.Name);
            }
        }

        while (true)
        {
            var added = 0;
            foreach (var module in modules.Values)
            {
                foreach (var target in module.Targets)
                {
                    dependencies[target].Add(module.Name);
                    foreach (var item in dependencies[module.Name])
                    {
                        if (!dependencies[target].Contains(item))
                        {
                            dependencies[target].Add(item);
                            added++;
                        }
                    }
                }
            }

            if (added == 0) break;
        }

        // Assume rx has single conjugation input, and it's inputs are all conjugations...
        var finalInputs = modules.Values.Where(_ =>
            _.Targets.Length is 1
            && _.Targets.Single() is not "rx"
            && modules[_.Targets.Single()].Targets.Contains("rx")).Select(_ => _.Name);

        var finalDependencies = finalInputs
            .ToDictionary(_ => _, _ => dependencies[_].Select(_ => modules[_]).ToArray())
            .ToFrozenDictionary();

        static string GetState(Module[] modules) =>
            modules.Aggregate(string.Empty, (_, module) => _ + module.GetState());

        var states = finalInputs.ToDictionary(_ => _, _ => new List<string> { GetState(finalDependencies[_]) });

        // Assume states all loop back to initial state
        // Assume every final input emits a high pulse at the end of the loop and low pulses otherwise
        var loops = finalInputs.ToDictionary(_ => _, _ => -1);
        for (var i = 1; true; i++)
        {
            PressButton(modules);

            finalDependencies.Where(_ => loops[_.Key] < 0).ForEach(_ =>
            {
                var state = GetState(_.Value);
                states[_.Key].Add(state);
                if (states[_.Key][0] == state)
                {
                    loops[_.Key] = i;
                }
            });

            if (loops.Values.All(_ => _ > 0)) break;
        }

        return loops.Values.Aggregate((long)1, (a, v) => a * v);
    }
}