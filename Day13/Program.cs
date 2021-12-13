using Common;

HashSet<Point> points = new HashSet<Point>();

var inputLines = File.ReadAllLines("input13.txt").ToList();

foreach (var line in inputLines.TakeWhile(l => l.Trim() != "")) {
    var parts = line.Split(',').ToArray();
    points.Add(new Point(int.Parse(parts[0]), int.Parse(parts[1])));
}

Console.WriteLine(points.Count());

foreach (var line in inputLines.SkipWhile(l => l.Trim() != "").Skip(1))
{
    var parts = line.Split('=').ToArray();
    int foldPos = int.Parse(parts[1]);
    if (parts[0].Last() == 'x')
    {
        foreach (var pos in points.Where(p => p.x > foldPos).ToList())
        {
            points.Add(new Point(pos.x - 2 * (pos.x - foldPos), pos.y));
            points.Remove(pos);
        }
    }
    if (parts[0].Last() == 'y')
    {
        foreach (var pos in points.Where(p => p.y > foldPos).ToList())
        {
            points.Add(new Point(pos.x, pos.y - 2 * (pos.y - foldPos)));
            points.Remove(pos);
        }
    }
    Console.WriteLine(points.Count());
}
Console.WriteLine();


static void Print(HashSet<Point> grid)
{
    int minY = grid.Min(p => p.y);
    int maxY = grid.Max(p => p.y);
    int minX = grid.Min(p => p.x);
    int maxX = grid.Max(p => p.x);
    for (int y = minY; y <= maxY; ++y)
    {
        for (int x = minX; x <= maxX; ++x)
        {
            Point pos = new Point(x, y);
            if (grid.Contains(pos))
            {
                Console.Write('#');
            }
            else
            {
                Console.Write(' ');
            }
        }
        Console.WriteLine();
    }
}

Print(points);

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}


// not 662