using Common;
using System.Diagnostics;
using System.Numerics;

string fileName = "../../../input.txt";
decimal minCoord = 200000000000000;
decimal maxCoord = 400000000000000;

//string fileName = "../../../testInput.txt";
//decimal minCoord = 7;
//decimal maxCoord = 27;

string[] lines = File.ReadAllLines(fileName);

Path[] paths = lines.Select(Path.Parse).ToArray();

int interestingCrossings = 0;
for (int i = 0; i < paths.Length; i++)
{
    for (int j = i + 1; j < paths.Length; j++)
    {
        Path a = paths[i];
        Path b = paths[j];
        if (a.CrossesInAreaInFuture(b, minCoord, maxCoord))
        {
            interestingCrossings++;
        }
    }
}

/*
Console.WriteLine(interestingCrossings);

Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 1, 2), paths[0], paths[1]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 1, 2), paths[1], paths[2]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 1, 2), paths[0], paths[2]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 1, 2), paths[0], paths[3]));

Console.WriteLine();
Console.WriteLine();


Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 2, 3), paths[0], paths[1]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 2, 3), paths[1], paths[2]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 2, 3), paths[0], paths[2]));
Console.WriteLine(Path.HitTwoParticlesFrom(new Point(-3, 2, 3), paths[0], paths[3]));
*/
long EuclidGCD(long a, long b)
{
    if (a == 0)
    {
        return b;
    }
    if (b == 0)
    {
        return a;
    }
    if (a > b)
    {
        return EuclidGCD(a % b, b);
    }
    return EuclidGCD(a, b % a);
}

IEnumerable<long> AllDivisors(long n)
{
    for (int i = 1; i * i <= n; i++)
    {
        if (n % i == 0)
        {
            yield return i;
            if (i * i != n)
            {
                yield return n / i;
            }
        }
    }
}

long findVelocityForDimension(IEnumerable<Progression1D> progressions)
{
    HashSet<long> possibleSteps = new HashSet<long>();
    IEnumerable<IGrouping<long, Progression1D>> groupedProgressions = progressions.GroupBy(p => p.Step);

    bool firstLargeGrouping = true;
    foreach (var g in groupedProgressions)
    {
        if (g.Count() > 2)
        {
            //Console.WriteLine($"X progression of {g.Key} has {g.Count()} paths");
            //Console.WriteLine("Their starting positions are:");
            foreach (var p in g)
            {
                //Console.WriteLine(p.Start);
            }
            long diff1 = Math.Abs(g.ElementAt(0).Start - g.ElementAt(1).Start);
            long diff2 = Math.Abs(g.ElementAt(1).Start - g.ElementAt(2).Start);
            if (diff1 == diff2 || diff1 == 0 || diff2 == 0)
            {
                //Console.WriteLine("Unhelpful differences");
            }
            else
            {
                HashSet<long> currentPossibleSteps = new HashSet<long>();
                //Console.WriteLine($"Differences are {diff1} and {diff2}");
                long gcd = EuclidGCD(diff1, diff2);
                //Console.WriteLine($"GCD is {gcd}");

                foreach (long divisor in AllDivisors(gcd))
                {
                    currentPossibleSteps.Add(divisor + g.Key);
                    currentPossibleSteps.Add(-divisor + g.Key);
                }
                //Console.WriteLine($"Current possibles count {currentPossibleSteps.Count}");
                //Console.WriteLine($"Previous possibles count {possibleSteps.Count}");
                if (firstLargeGrouping)
                {

                    possibleSteps = currentPossibleSteps.ToHashSet();
                    firstLargeGrouping = false;
                }
                else
                {
                    possibleSteps.IntersectWith(currentPossibleSteps);
                }
                Debug.Assert(possibleSteps.Count > 0);
                //Console.WriteLine($"Count after combining {possibleSteps.Count}");
            }
        }
    }
    return possibleSteps.Single();
}

long dx = findVelocityForDimension(paths.Select(p => p.XProgression()));
long dy = findVelocityForDimension(paths.Select(p => p.YProgression()));
long dz = findVelocityForDimension(paths.Select(p => p.ZProgression()));

//long dx = -3;
//long dy = 1;
//long dz = 2;

Console.WriteLine($"It would appear the valid X velocity is {dx}");
Console.WriteLine($"It would appear the valid Y velocity is {dy}");
Console.WriteLine($"It would appear the valid Z velocity is {dz}");

BigInteger prevmod = 0;
BigInteger prevremainder = 0;

for (int i = 0; i < paths.Length; i++)
{
    BigInteger remainder = (long)paths[i].Start.X;
    BigInteger mod = Math.Abs((long)paths[i].Velocity.X - dx);

    if (mod == 0)
    {
        continue;
    }

    remainder = remainder % mod;

//    Console.WriteLine($"Looking at path {i}, We think x == {remainder % mod} mod {mod}");
    if (prevmod == 0)
    {
        prevmod = mod;
        prevremainder = remainder;
    }
    else
    {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(prevmod, mod);
        if (gcd != 1)
        {
//            Console.WriteLine($"GCD of {prevmod} and {mod} is {gcd}, ignoring");
            continue;
        }

        BigInteger candidateRemainder = prevremainder;
        while (candidateRemainder % mod != remainder % mod)
        {
            candidateRemainder += prevmod;
        }
        prevmod = prevmod * mod;
        prevremainder = candidateRemainder;
        Console.WriteLine($"We now think x == {prevremainder} mod {prevmod}");
    }
}
BigInteger total = prevremainder;


prevmod = 0;
prevremainder = 0;

for (int i = 0; i < paths.Length; i++)
{
    BigInteger remainder = (long)paths[i].Start.Y;
    BigInteger mod = Math.Abs((long)paths[i].Velocity.Y - dy);

    if (mod == 0)
    {
        continue;
    }

    remainder = remainder % mod;

//    Console.WriteLine($"Looking at path {i}, We think y == {remainder % mod} mod {mod}");
    if (prevmod == 0)
    {
        prevmod = mod;
        prevremainder = remainder;
    }
    else
    {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(prevmod, mod);
        if (gcd != 1)
        {
//            Console.WriteLine($"GCD of {prevmod} and {mod} is {gcd}, ignoring");
            continue;
        }

        BigInteger candidateRemainder = prevremainder;
        while (candidateRemainder % mod != remainder % mod)
        {
            candidateRemainder += prevmod;
        }
        prevmod = prevmod * mod;
        prevremainder = candidateRemainder;
        Console.WriteLine($"We now think y == {prevremainder} mod {prevmod}");
    }
}
total += prevremainder;


prevmod = 0;
prevremainder = 0;

for (int i = 0; i < paths.Length; i++)
{
    BigInteger remainder = (long)paths[i].Start.Z;
    BigInteger mod = Math.Abs((long)paths[i].Velocity.Z - dz);

    if (mod == 0)
    {
        continue;
    }

    remainder = remainder % mod;

 //   Console.WriteLine($"Looking at path {i}, We think z == {remainder % mod} mod {mod}");
    if (prevmod == 0)
    {
        prevmod = mod;
        prevremainder = remainder;
    }
    else
    {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(prevmod, mod);
        if (gcd != 1)
        {
//            Console.WriteLine($"GCD of {prevmod} and {mod} is {gcd}, ignoring");
            continue;
        }

        BigInteger candidateRemainder = prevremainder;
        while (candidateRemainder % mod != remainder % mod)
        {
            candidateRemainder += prevmod;
        }
        prevmod = prevmod * mod;
        prevremainder = candidateRemainder;
        Console.WriteLine($"We now think z == {prevremainder} mod {prevmod}");
    }
}
total += prevremainder;

Console.WriteLine(total);

public record Point(decimal X, decimal Y, decimal Z)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Point operator*(decimal a, Point b)
    {
        return new Point(a * b.X, a * b.Y, a * b.Z);
    }

    public bool LooksIntegral()
    {
        decimal eps = 0.0001m;
        return Math.Abs(X - Math.Round(X)) < eps && Math.Abs(Y - Math.Round(Y)) < eps && Math.Abs(Z - Math.Round(Z)) < eps;
    }
}

public record Progression1D(long Start, long Step)
{
}

public record Path(Point Start, Point Velocity)
{
    public static Path Parse(string line)
    {
        char[] separators = new char[] { ',', '@' };
        string[] parts = line.Split(separators, StringSplitOptions.TrimEntries);

        Point start = new Point(decimal.Parse(parts[0]), decimal.Parse(parts[1]), decimal.Parse(parts[2]));
        Point velocity = new Point(decimal.Parse(parts[3]), decimal.Parse(parts[4]), decimal.Parse(parts[5]));
        return new Path(start, velocity);
    }

    public Progression1D XProgression()
    {
        return new Progression1D((long)Start.X, (long)Velocity.X);
    }

    public Progression1D YProgression()
    {
        return new Progression1D((long)Start.Y, (long)Velocity.Y);
    }

    public Progression1D ZProgression()
    {
        return new Progression1D((long)Start.Z, (long)Velocity.Z);
    }

    public bool CrossesInAreaInFuture(Path other, decimal minCoord, decimal maxCoord)
    {
        decimal t1 = Velocity.X;
        decimal t2 = Velocity.Y;
        decimal s1 = - other.Velocity.X;
        decimal s2 = - other.Velocity.Y;
        decimal c1 = other.Start.X - Start.X;
        decimal c2 = other.Start.Y - Start.Y;

        decimal t3 = t2;
        decimal s3 = s2;
        decimal c3 = c2;

        if (t2 == 0)
        {
            (t1, t2) = (t2, t1);
            (s1, s2) = (s2, s1);
            (c1, c2) = (c2, c1);
        }
        else
        {
            decimal m1 = -t2 / t1;

            t3 = t2 + m1 * t1;
            s3 = s2 + m1 * s1;
            c3 = c2 + m1 * c1;
        }
        Debug.Assert(Math.Abs(t3) < 0.000001m);

        decimal t4 = t1;
        decimal s4 = s1;
        decimal c4 = c1;
        if (s3 == 0)
        {
//            Console.WriteLine($"Parallel paths?: {this} {other}");
        }
        else
        {
            decimal m2 = -s1 / s3;
            t4 = t1 + m2 * t3;
            s4 = s1 + m2 * s3;
            c4 = c1 + m2 * c3;
        }
        if (Math.Abs(s4) > 0.000001m)
        {
//            Console.WriteLine("Returning false for supposedly parallel lines");
            return false;
        }

        decimal time1 = c4 / t4;
        decimal time2 = c3 / s3;

        if (time1 < 0 || time2 < 0)
        {
            return false;
        }

        decimal x = Start.X + Velocity.X * time1;
        decimal y = Start.Y + Velocity.Y * time1;

        if (x < minCoord || x > maxCoord || y < minCoord || y > maxCoord)
        {
            return false;
        }
        return true;
    }

    private static List<decimal> SolveLinearSystem(List<List<decimal>> matrix)
    {
        int n = matrix.Count;
        for (int i = 0; i < n; i++)
        {
            // Search for maximum in this column, probably more stable numerically?
            decimal maxEl = Math.Abs(matrix[i][i]);
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(matrix[k][i]) > maxEl)
                {
                    maxEl = Math.Abs(matrix[k][i]);
                    maxRow = k;
                }
            }

            for (int k = i; k < n + 1; k++)
            {
                decimal tmp = matrix[maxRow][k];
                matrix[maxRow][k] = matrix[i][k];
                matrix[i][k] = tmp;
            }

            for (int k = i + 1; k < n; k++)
            {
                // Make all rows below this one 0 in current column
                decimal c = -matrix[k][i] / matrix[i][i];
                for (int j = i + 1; j < n + 1; j++)
                {
                    if (i == j)
                    {
                        matrix[k][j] = 0;
                    }
                    else
                    {
                        matrix[k][j] += c * matrix[i][j];
                    }
                }
            }
        }
        // find values
        for (int i = 0; i < n; i++)
        {
            if (matrix[i][i] == 0)
            {
                throw new Exception("Singular Matrix");
            }
            matrix[i][n] /= matrix[i][i];
        }
        // back substitution
        List<decimal> x = new List<decimal>(n);
        for (int i = n - 1; i >= 0; i--)
        {
            x.Add(matrix[i][n]);
            for (int k = i - 1; k >= 0; k--)
            {
                matrix[k][n] -= matrix[k][i] * x[n - 1 - i];
            }
        }
        x.Reverse();
        return x;
    }


    public static Point HitTwoParticlesFrom(Point vel, Path a, Path b)
    {
        // x0, y0, z0, tcol1, tcol2
        List<List<decimal>> matrix = new List<List<decimal>>();

        // x0 + vx * tcol1 = a.Start.X + a.Velocity.X * tcol1
        matrix.Add(new List<decimal>() { 1, 0, 0, vel.X - a.Velocity.X, 0, a.Start.X });
        // y0 + vy * tcol1 = a.Start.Y + a.Velocity.Y * tcol1
        matrix.Add(new List<decimal>() { 0, 1, 0, vel.Y - a.Velocity.Y, 0, a.Start.Y });
        // z0 + vz * tcol1 = a.Start.Z + a.Velocity.Z * tcol1
        matrix.Add(new List<decimal>() { 0, 0, 1, vel.Z - a.Velocity.Z, 0, a.Start.Z });
        // x0 + vx * tcol2 = b.Start.X + b.Velocity.X * tcol2
        matrix.Add(new List<decimal>() { 1, 0, 0, 0, vel.X - b.Velocity.X, b.Start.X });
        // y0 + vy * tcol2 = b.Start.Y + b.Velocity.Y * tcol2
        matrix.Add(new List<decimal>() { 0, 1, 0, 0, vel.Y - b.Velocity.Y, b.Start.Y });
        // z0 + vz * tcol2 = b.Start.Z + b.Velocity.Z * tcol2
        // not making it overdetermined
        //matrix.Add(new List<decimal>() { 0, 0, 1, 0, vel.Z - b.Velocity.Z, -b.Start.Z });
        try
        {
            var result = SolveLinearSystem(matrix);
            Point start = new Point(result[0], result[1], result[2]);
            decimal tcol1 = result[3];
            decimal tcol2 = result[4];
            /*        Console.WriteLine("===========================================");
                    Console.WriteLine($"Testing with velocity {vel}");

                    Console.WriteLine($"Hailstone {a}");
                    Console.WriteLine($"Collision time: {tcol1}");
                    Console.WriteLine($"Collision position: {start + tcol1 * vel}");

                    Console.WriteLine($"Hailstone {b}");
                    Console.WriteLine($"Collision time: {tcol2}");
                    Console.WriteLine($"Collision position: {start + tcol2 * vel}");
                    Console.WriteLine($"Starting position: {start}");*/
            return start;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public bool Hit(Point start, Point vel)
    {
        if (vel.X == Velocity.X)
        {
            return start == Start;
        }
        decimal tcol = (Start.X - start.X) / (vel.X - Velocity.X);
        if (tcol < 0)
        {
            return false;
        }
        Point collision = start + tcol * vel;
        return Math.Abs(collision.Y - (Start.Y + tcol * Velocity.Y)) + Math.Abs(collision.Z - (Start.Z + tcol * Velocity.Z)) < 0.000001m;
    }
}
