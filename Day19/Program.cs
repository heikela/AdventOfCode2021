using Common;

// See https://aka.ms/new-console-template for more information

HashSet<Point> ParseScanner(IEnumerable<string> lines)
{
    HashSet<Point> points = new HashSet<Point>();
    foreach (string line in lines.Skip(1))
    {
        int[] parts = line.Split(",").Select(int.Parse).ToArray();
        points.Add(new Point(parts[0], parts[1], parts[2]));
    }
    return points;
}

var scanners = File.ReadLines("input19.txt").Paragraphs().Select(lines => ParseScanner(lines));

Console.WriteLine(scanners.Count());

Point rotateAndMirror(Point a, Point b)
{
    if (Math.Abs(b.x) != 1 || Math.Abs(b.y) != 1 || Math.Abs(b.z) != 1)
    {
        throw new ArgumentException("Expected second argument to reotateAndMirror to have -1 or 1 on each axis");
    }
    return new Point(a.x * b.x, a.y * b.y, a.z * b.z);
}
public record Point(int x, int y, int z)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y, a.z - b.z);
}

public record AxisTransformation(int rotate, int translate)
{
    public int Apply(int x) {
        return this.rotate * x + this.translate;
    }
}
