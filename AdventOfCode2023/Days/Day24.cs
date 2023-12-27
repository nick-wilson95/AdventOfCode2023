
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

    // Assumptions:
    //  No velocity has a prime factor > 907
    //  Stone direction vector components are <= than 500
    //  Stone direction components dont have any repeat prime factors (performance work needed to scan more prime combinations)
    // Note:
    //  I copied the LinearEquationSolver used in this solution from the internet for expediency
    public static object SolvePart2(ImmutableArray<string> input)
    {
        var hailstones = input.Select(Hailstone.Parse).ToList();

        var (h0CollisionTime, stoneDirection) = GetStoneDirection(hailstones);

        var pointOfContact = hailstones[0].PositionAt((long)h0CollisionTime);

        return pointOfContact.X - h0CollisionTime * stoneDirection.X
            + (pointOfContact.Y - h0CollisionTime * stoneDirection.Y)
            + (pointOfContact.Z - h0CollisionTime * stoneDirection.Z);
    }

    private static (double h0CollisionTime, Vector velocity) GetStoneDirection(List<Hailstone> hailstones)
    {
        // Given prime p:
        //  If there exist hailstones G, H such that:
        //      (G.xt | p) and (H.xt | p) are constant in t and not equal (where xt is x position at time t)
        //  Then stone.velocity.x cannot have p as a prime factor
        // Reflected statements hold for y and z components

        var invariantX = GetInvariantPrimes(hailstones, _ => _.X);
        var invariantY = GetInvariantPrimes(hailstones, _ => _.Y);
        var invariantZ = GetInvariantPrimes(hailstones, _ => _.Z);

        var tryX = GetValuesByPrimes(invariantX).ToArray();
        var tryY = GetValuesByPrimes(invariantY).ToArray();
        var tryZ = GetValuesByPrimes(invariantZ).ToArray();

        foreach (var x in tryX)
        {
            foreach (var y in tryY)
            {
                foreach (var z in tryZ)
                {

                    var tests = new[]
                    {
                        TestVelocity(hailstones, new( x,  y,  z)),
                        TestVelocity(hailstones, new(-x,  y,  z)),
                        TestVelocity(hailstones, new( x, -y,  z)),
                        TestVelocity(hailstones, new( x,  y, -z)),
                    };

                    if (tests.Any(_ => _.works))
                    {
                        var result = tests.Single(_ => _.works);
                        return (result.h0CollisionTime, result.direction);
                    }
                }
            }
        }

        throw new Exception("Failed to find direction");
    }

    private static IEnumerable<int> GetValuesByPrimes(int[] primes)
    {
        const int Limit = 500;

        var primeCounts = Math.Pow(2, primes.Length);
        for (var i = 0; i < primeCounts; i++)
        {
            double value = 1;
            var current = i;
            for (var j = 0; j < primes.Length; j++)
            {
                if (current is 0) break;
                value *= Math.Pow(primes[j], current % 2);
                if (value > Limit) break;
                current /= 2;
            }

            if (value > Limit) continue;

            yield return (int)value;
        }
    }

    private static (bool works, double h0CollisionTime, Vector direction) TestVelocity(List<Hailstone> hailstones, Vector velocity)
    {
        static double GetH0CollisionTime(Hailstone h0, Hailstone h1, Vector direction)
        {
            var result = LinearEquationSolver.Solve([
                [h0.Velocity.X, direction.X, -h1.Velocity.X, h1.Position.X - h0.Position.X],
                [h0.Velocity.Y, direction.Y, -h1.Velocity.Y, h1.Position.Y - h0.Position.Y],
                [h0.Velocity.Z, direction.Z, -h1.Velocity.Z, h1.Position.Z - h0.Position.Z]
            ]);

            return result[0];
        }

        var time1 = GetH0CollisionTime(hailstones[0], hailstones[1], velocity);
        var time2 = GetH0CollisionTime(hailstones[0], hailstones[2], velocity);

        return (time1 == time2, time1, velocity);
    }


    private static int[] GetInvariantPrimes(List<Hailstone> hailstones, Func<Vector, double> GetValue)
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
}