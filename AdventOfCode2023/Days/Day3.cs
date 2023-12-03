using System.Buffers;

namespace AdventOfCode2023.Solutions;

public record Day3 : Day<Day3>, IDay<Day3>
{
    public static int DayNumber => 3;

    private static readonly SearchValues<char> Digits = SearchValues.Create(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']);

    public static object SolvePart1(ImmutableArray<string> input)
    {
        static bool IsSymbol(char c) => c is not '.' && !Digits.Contains(c);

        bool SymbolAdjacent(int row, int startCol, int numDigits)
        {
            if (row > 0 && input[row - 1].Substring(startCol, numDigits).Any(IsSymbol))
            {
                return true;
            }
            if (row < input.Length - 1 && input[row + 1].Substring(startCol, numDigits).Any(IsSymbol))
            {
                return true;
            }

            var otherAdjacentPoints = new (int i, int j)[]
            {
                (row - 1, startCol - 1),
                (row, startCol - 1),
                (row + 1, startCol - 1),
                (row - 1, startCol + numDigits),
                (row, startCol + numDigits),
                (row + 1, startCol + numDigits)
            };

            foreach (var (i, j) in otherAdjacentPoints)
            {
                if (i < 0 || j < 0 || i >= input.Length || j >= input[0].Length) continue;
                if (IsSymbol(input[i][j])) return true;
            }

            return false;
        }


        int total = 0;
        ForEachNumber(input, (position, digits, value) =>
        {
            if (SymbolAdjacent(position.i, position.j - digits.Length, digits.Length))
            {
                total += value;
            }
        });

        return total;
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        IEnumerable<(int i, int j)> GetAdjacentGears(int row, int startCol, int numDigits)
        {
            for (var j = startCol; j < startCol + numDigits; j++)
            {
                if (row > 0 && input[row - 1][j] is '*')
                {
                    yield return (row - 1, j);
                }
                if (row < input.Length - 1 && input[row + 1][j] is '*')
                {
                    yield return (row + 1, j);
                }
            }

            var otherAdjacentPoints = new (int i, int j)[]
            {
                (row - 1, startCol - 1),
                (row, startCol - 1),
                (row + 1, startCol - 1),
                (row - 1, startCol + numDigits),
                (row, startCol + numDigits),
                (row + 1, startCol + numDigits)
            };

            foreach (var (i, j) in otherAdjacentPoints)
            {
                if (i < 0 || j < 0 || i >= input.Length || j >= input[0].Length) continue;
                if (input[i][j] is '*') yield return (i, j);
            }
        }

        var gearNumbers = new Dictionary<(int i, int j), List<int>>();
        ForEachNumber(input, (position, digits, value) =>
        {
            foreach (var (i, j) in GetAdjacentGears(position.i, position.j - digits.Length, digits.Length))
            {
                if (gearNumbers.ContainsKey((i, j)))
                {
                    gearNumbers[(i, j)].Add(value);
                }
                else
                {
                    gearNumbers[(i, j)] = [value];
                }
            }
        });

        return gearNumbers.Values.Where(_ => _.Count is 2).Sum(_ => _[0] * _[1]);
    }

    public static void ForEachNumber(ImmutableArray<string> input, Action<(int i, int j), string, int> action)
    {
        for (var i = 0; i < input.Length; i++)
        {
            string currentNum = "";
            var rowPlusDot = input[i] + '.';
            for (var j = 0; j < rowPlusDot.Length; j++)
            {
                var character = rowPlusDot[j];
                if (Digits.Contains(character))
                {
                    currentNum += character;
                }
                else if (currentNum.Length > 0)
                {
                    var number = int.Parse(currentNum);
                    action.Invoke((i, j), currentNum, number);
                    currentNum = "";
                }
            }
        }
    }
}