namespace AdventOfCode2023.Solutions;

public record Day24 : Day<Day24>, IDay<Day24>
{
    public static int DayNumber => 24;

    private record Vector(double X, double Y, double Z)
    {
        public static Vector Parse(string str) => str.Split(", ").Select(long.Parse).Fold((a, b, c) => new Vector(a, b, c));

        public Vector Minus(Vector other) => new(X - other.X, Y - other.Y, Z - other.Z);

        public double Length => Math.Pow(Math.Pow(X, 3) + Math.Pow(Y, 3) + Math.Pow(Z, 3), 1/3);
    }

    private record Hailstone(Vector Position, Vector Velocity)
    {
        public static Hailstone Parse(string line) => line.Split(" @ ").Select(Vector.Parse).Fold((a, b) => new Hailstone(a, b));

        public Vector PositionAt(long time) => new(
            Position.X + time * Velocity.X,
            Position.Y + time * Velocity.Y,
            Position.Z + time * Velocity.Z);
    }

    public static object SolvePart1(ImmutableArray<string> input)
    {
        const long CMin = 200000000000000;
        const long CMax = 400000000000000;

        var hailstones = input.Select(Hailstone.Parse).ToList();

        var colllisions = 0;
        for (var i = 0; i < hailstones.Count; i++)
        {
            var hA = hailstones[i];
            for (var j = i + 1; j < hailstones.Count; j++)
            {
                var hB = hailstones[j];

                if (hA.Velocity.X * hB.Velocity.Y == hB.Velocity.X * hA.Velocity.Y)
                {
                    continue; // Parallel
                }

                var t = (((hB.Position.X - hA.Position.X) / hA.Velocity.X) - ((hB.Position.Y - hA.Position.Y) / hA.Velocity.Y))
                    / (hB.Velocity.Y / hA.Velocity.Y - hB.Velocity.X / hA.Velocity.X);

                if (t < 0) continue;

                var collisionX = hB.Position.X + hB.Velocity.X * t;
                var collisionY = hB.Position.Y + hB.Velocity.Y * t;

                var s = (collisionX - hA.Position.X) / hA.Velocity.X;

                if (s < 0) continue;
                if (collisionX >= CMin && collisionX <= CMax && collisionY >= CMin && collisionY <= CMax)
                {
                    colllisions++;
                }
            }
        }

        return colllisions;
    }

    public static object SolvePart2(ImmutableArray<string> input)
    {
        var hailstones = input.Select(Hailstone.Parse).ToList();

        var variantX = GetVariantPrimes(hailstones, _ => _.X);
        var variantY = GetVariantPrimes(hailstones, _ => _.Y);
        var variantZ = GetVariantPrimes(hailstones, _ => _.Z);

        // Variants can't be prime factors of direction of travel components

        List<(bool works, double h1Time, Vector direction)> results = [];
        void Test(int a, int b, int c)
        {
            results.AddRange([
                TestLine(hailstones, new(a * 1, b * 1, c * 1)),
                TestLine(hailstones, new(a * -1, b * 1, c * 1)),
                TestLine(hailstones, new(a * 1, b * -1, c * 1)),
                TestLine(hailstones, new(a * 1, b * 1, c * -1)),
                TestLine(hailstones, new(a * 3, b * 2, c * 1)),
                TestLine(hailstones, new(a * -3, b * 2, c * 1)),
                TestLine(hailstones, new(a * 3, b * -2, c * 1)),
                TestLine(hailstones, new(a * 3, b * 2, c * -1)),
                TestLine(hailstones, new(a * 3, b * 4, c * 1)),
                TestLine(hailstones, new(a * -3, b * 4, c * 1)),
                TestLine(hailstones, new(a * 3, b * -4, c * 1)),
                TestLine(hailstones, new(a * 3, b * 4, c * -1)),
                TestLine(hailstones, new(a * 3, b * 8, c * 1)),
                TestLine(hailstones, new(a * -3, b * 8, c * 1)),
                TestLine(hailstones, new(a * 3, b * -8, c * 1)),
                TestLine(hailstones, new(a * 3, b * 8, c * -1)),
                TestLine(hailstones, new(a * 9, b * 2, c * 1)),
                TestLine(hailstones, new(a * -9, b * 2, c * 1)),
                TestLine(hailstones, new(a * 9, b * -2, c * 1)),
                TestLine(hailstones, new(a * 9, b * 2, c * -1)),
                TestLine(hailstones, new(a * 9, b * 4, c * 1)),
                TestLine(hailstones, new(a * -9, b * 4, c * 1)),
                TestLine(hailstones, new(a * 9, b * -4, c * 1)),
                TestLine(hailstones, new(a * 9, b * 4, c * -1)),
                TestLine(hailstones, new(a * 9, b * 8, c * 1)),
                TestLine(hailstones, new(a * -9, b * 8, c * 1)),
                TestLine(hailstones, new(a * 9, b * -8, c * 1))
            ]);
        }

        Test(1, 1, 1);
        Test(67, 1, 1);
        Test(1, 101, 1);
        Test(1, 1, 79);
        Test(67, 101, 1);
        Test(67, 1, 79);
        Test(1, 101, 79);
        Test(67, 101, 79);

        var (_, h1Time, direction) = results.Single(_ => _.works);

        var pointOfContact = hailstones[0].PositionAt((long)h1Time);

        return pointOfContact.X - h1Time * direction.X
            + (pointOfContact.Y - h1Time * direction.Y)
            + (pointOfContact.Z - h1Time * direction.Z);
    }

    private static (bool works, double h1Time, Vector direction) TestLine(List<Hailstone> hailstones, Vector direction)
    {
        static double GetH1Time(Hailstone h1, Hailstone h2, Vector direction)
        {
            var result = SolveLinearEquations([
                [h1.Velocity.X, direction.X, -h2.Velocity.X, h2.Position.X - h1.Position.X],
                [h1.Velocity.Y, direction.Y, -h2.Velocity.Y, h2.Position.Y - h1.Position.Y],
                [h1.Velocity.Z, direction.Z, -h2.Velocity.Z, h2.Position.Z - h1.Position.Z]
            ]);

            return result[0];
        }

        var time1 = GetH1Time(hailstones[0], hailstones[1], direction);
        var time2 = GetH1Time(hailstones[0], hailstones[2], direction);

        return (time1 == time2, time1, direction);
    }


    private static int[] GetVariantPrimes(List<Hailstone> hailstones, Func<Vector, double> GetValue)
    {
        var primes = hailstones.SelectMany(_ => GetPrimeFactors((long)GetValue(_.Velocity)).Select(p => (v: GetValue(_.Position) % p, p)))
            .GroupBy(_ => _.p)
            .OrderBy(_ => _.Key)
            .ToDictionary(_ => _.Key, _ => _.Select(v => v.v).ToArray());

        return primes.Where(_ => _.Value.Distinct().Count() == 1).Select(_ => _.Key).ToArray();
    }

    static readonly int[] Primes =
    [
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
        73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
        179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
        283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
        419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
        547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
        661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
        811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907
    ];

    public static HashSet<int> GetPrimeFactors(long number)
    {
        var current = number;
        var result = new HashSet<int>();
        foreach (var prime in Primes)
        {
            while (current % prime is 0)
            {
                current /= prime;
                result.Add(prime);
            }
        }
        return result;
    }

    private static double[] SolveLinearEquations(double[][] rows)
    {

        int length = rows[0].Length;

        for (int i = 0; i < rows.Length - 1; i++)
        {
            if (rows[i][i] == 0 && !Swap(rows, i, i))
            {
                return null;
            }

            for (int j = i; j < rows.Length; j++)
            {
                double[] d = new double[length];
                for (int x = 0; x < length; x++)
                {
                    d[x] = rows[j][x];
                    if (rows[j][i] != 0)
                    {
                        d[x] = d[x] / rows[j][i];
                    }
                }
                rows[j] = d;
            }

            for (int y = i + 1; y < rows.Length; y++)
            {
                double[] f = new double[length];
                for (int g = 0; g < length; g++)
                {
                    f[g] = rows[y][g];
                    if (rows[y][i] != 0)
                    {
                        f[g] = f[g] - rows[i][g];
                    }

                }
                rows[y] = f;
            }
        }

        return CalculateResult(rows);
    }

    private static bool Swap(double[][] rows, int row, int column)
    {
        bool swapped = false;
        for (int z = rows.Length - 1; z > row; z--)
        {
            if (rows[z][row] != 0)
            {
                double[] temp = new double[rows[0].Length];
                temp = rows[z];
                rows[z] = rows[column];
                rows[column] = temp;
                swapped = true;
            }
        }

        return swapped;
    }
    private static double[] CalculateResult(double[][] rows)
    {
        double val = 0;
        int length = rows[0].Length;
        double[] result = new double[rows.Length];
        for (int i = rows.Length - 1; i >= 0; i--)
        {
            val = rows[i][length - 1];
            for (int x = length - 2; x > i - 1; x--)
            {
                val -= rows[i][x] * result[x];
            }
            result[i] = val / rows[i][i];

            if (!IsValidResult(result[i]))
            {
                return null;
            }
        }
        return result;
    }

    private static bool IsValidResult(double result)
    {
        return !(double.IsNaN(result) || double.IsInfinity(result));
    }
}