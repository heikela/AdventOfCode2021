using Common;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

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

Console.WriteLine(interestingCrossings);


public record Point(decimal X, decimal Y)
{
    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}

public record Path(Point Start, Point Velocity)
{
    public static Path Parse(string line)
    {
        char[] separators = new char[] { ',', '@' };
        string[] parts = line.Split(separators, StringSplitOptions.TrimEntries);

        Point start = new Point(decimal.Parse(parts[0]), decimal.Parse(parts[1]));
        Point velocity = new Point(decimal.Parse(parts[3]), decimal.Parse(parts[4]));
        return new Path(start, velocity);
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
            Console.WriteLine($"Parallel paths?: {this} {other}");
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
            Console.WriteLine("Returning false for supposedly parallel lines");
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
}
