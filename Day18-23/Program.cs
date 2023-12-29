using Common;

string[] lines = new AoCUtil().GetInput(2023, 18);
//string[] lines = new AoCUtil().GetTestBlock(2023, 18, 0);

Point up = new Point(0, -1);
Point down = new Point(0, 1);
Point left = new Point(-1, 0);
Point right = new Point(1, 0);

(Point, int) parseLine1(string line)
{
    string[] parts = line.Split(' ');
    char dirChar = parts[0].First();
    Point dir = dirChar switch
    {
        'U' => up,
        'D' => down,
        'L' => left,
        'R' => right,
        _ => throw new Exception($"Invalid direction {dirChar}")
    };
    int steps = Int32.Parse(parts[1]);
    return (dir, steps);
}

(Point, int) parseLine2(string line)
{
    string[] parts = line.Split(' ');
    char dirChar = parts[2].Reverse().Skip(1).First();
    Point dir = dirChar switch
    {
        '3' => up,
        '1' => down,
        '2' => left,
        '0' => right,
        _ => throw new Exception($"Invalid direction {dirChar}")
    };
    int steps = Int32.Parse(parts[2].Substring(2, parts[2].Length - 4), System.Globalization.NumberStyles.HexNumber);
    return (dir, steps);
}

Point nextPoint(Point current, (Point, int) move)
{
    Point dir = move.Item1;
    int steps = move.Item2;
    return current + dir * steps;
}

var moves = lines.Select(parseLine2).ToList();
var corners = moves.ProcessAdjacent(new Point(0, 0), nextPoint);

long innerArea = polyArea(corners);
long edgeContribution = edgeArea(moves.Select(m => m.Item2));

Console.WriteLine(innerArea + edgeContribution);


long polyArea(IEnumerable<Point> poly)
{
    Point prev = poly.Last();
    long sum1 = 0;
    long sum2 = 0;
    foreach (Point p in poly)
    {
        sum1 += (long)prev.X * (long)p.Y;
        sum2 += (long)prev.Y * (long)p.X;
        prev = p;
    }
    return Math.Abs(sum1 - sum2) / 2;
}

long edgeArea(IEnumerable<int> moveLengths)
{
    long sum = 0;
    foreach (int length in moveLengths)
    {
        sum += length;
    }
    return sum / 2 + 1; // this assumes the edge does not cross itself
}

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public static Point operator*(Point a, int b) => new Point(a.X * b, a.Y * b);
}