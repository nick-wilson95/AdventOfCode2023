namespace AdventOfCode2023.Solutions;

public record Day15 : Day<Day15>, IDay<Day15>
{
    public static int DayNumber => 15;

    private static int Hash(string chars) => chars.Aggregate(0, (v, c) => (17 * (v + c)) % 256);

    public static object SolvePart1(ImmutableArray<string> input) => input.Single().Split(',').Sum(Hash);

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var boxes = Enumerable.Range(0, 256).ToDictionary(_ => _, _ => new List<(string Label, int Lens)>());
        foreach (var step in input.Single().Split(','))
        {
            var (label, lens) = step.Split('=', '-').Fold((a, b) => (a, b.ParseInt()));
            var box = boxes[Hash(label)];
            var currentIndex = box.FindIndex(_ => _.Label == label);

            if (lens.HasValue)
            {
                if (currentIndex is -1) box.Add((label, lens.Value));
                else box[currentIndex] = (label, lens.Value);
            }
            else if (currentIndex is not -1) box.RemoveAt(currentIndex);
        }

        return boxes.Sum(box => (box.Key + 1) * box.Value.Index().Sum(_ => (_.Key + 1) * _.Value.Lens));
    }
}