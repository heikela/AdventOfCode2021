using Common;

// See https://aka.ms/new-console-template for more information

List<Rotation> rotations = new List<Rotation>()
{
    new Rotation(1, 0, 0, 0, 1, 0, 0, 0, 1),
    new Rotation(1, 0, 0, 0, 0, 1, 0, -1, 0),
    new Rotation(1, 0, 0, 0, -1, 0, 0, 0, -1),
    new Rotation(1, 0, 0, 0, 0, -1, 0, 1, 0),

    new Rotation(-1, 0, 0, 0, 1, 0, 0, 0, -1),
    new Rotation(-1, 0, 0, 0, 0, 1, 0, 1, 0),
    new Rotation(-1, 0, 0, 0, -1, 0, 0, 0, 1),
    new Rotation(-1, 0, 0, 0, 0, -1, 0, -1, 0),

    new Rotation(0, 0, 1, 0, 1, 0, -1, 0, 0),
    new Rotation(0, 0, 1, 1, 0, 0, 0, 1, 0),
    new Rotation(0, 0, 1, 0, -1, 0, 1, 0, 0),
    new Rotation(0, 0, 1, -1, 0, 0, 0, -1, 0),

    new Rotation(0, 0, -1, 0, 1, 0, 1, 0, 0),
    new Rotation(0, 0, -1, 1, 0, 0, 0, -1, 0),
    new Rotation(0, 0, -1, 0, -1, 0, -1, 0, 0),
    new Rotation(0, 0, -1, -1, 0, 0, 0, 1, 0),

    new Rotation(0, 1, 0, -1, 0, 0, 0, 0, -1),
    new Rotation(0, 1, 0, 0, 0, -1, 1, 0, 0),
    new Rotation(0, 1, 0, 1, 0, 0, 0, 0, 1),
    new Rotation(0, 1, 0, 0, 0, 1, -1, 0, 0),

    new Rotation(0, -1, 0, -1, 0, 0, 0, 0, 1),
    new Rotation(0, -1, 0, 0, 0, -1, -1, 0, 0),
    new Rotation(0, -1, 0, 1, 0, 0, 0, 0, -1),
    new Rotation(0, -1, 0, 0, 0, 1, 1, 0, 0),
};

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

(Boolean matched, HashSet<Point> combined) CombineAreas(HashSet<Point> a, HashSet<Point> b)
{
    HashSet<Transformation> transformations = new HashSet<Transformation>();
    foreach (Rotation rotation in rotations) {
        for (int xTranslate = -2000; xTranslate <= 2000; ++xTranslate)
        {
            for (int yTranslate = -2000; yTranslate <= 2000; ++yTranslate)
            {
                for (int zTranslate = -2000; zTranslate <= 2000; ++zTranslate)
                {
                    Point translation = new Point(xTranslate, yTranslate, zTranslate);
                    int matchCount = 0;
                    foreach (Point p in b)
                    {
                        Point transformedP = rotate(p, rotation) + translation;
                        if (a.Contains(transformedP))
                        {
                            matchCount++;
                        }
                    }
                    if (matchCount >= 12)
                    {
                        transformations.Add(new Transformation(rotation, translation));
                    }
                }
            }
        }
    }
    if (transformations.Count == 1)
    {
        Transformation t = transformations.Single();
        HashSet<Point> combined = a.ToHashSet();
        foreach (Point p in b)
        {
            combined.Add(rotate(p, t.rot) + t.translation);
        }
        return (true, combined);
    }
    else if (transformations.Count > 1)
    {
        throw new Exception("Found multiple matching transformations");
    }
    else
    {
        return (false, null);
    }
}

var scanners = File.ReadLines("input19.txt").Paragraphs().Select(lines => ParseScanner(lines)).ToList();

Console.WriteLine(scanners.Count());

while (scanners.Count > 1)
{
    for (int i = 0; i < scanners.Count - 1; ++i)
    {
        for (int j = i + 1; j < scanners.Count; ++j)
        {
            (Boolean combined, HashSet<Point> result) = CombineAreas(scanners[i], scanners[j]);
            if (combined) {
                scanners.RemoveAt(i);
                scanners.RemoveAt(j);
                scanners.Add(result);
            }
        }
    }
}

Console.WriteLine($"In total there are {scanners.Single().Count} beacons");

Point rotate(Point a, Rotation r)
{
    if (Math.Abs(r.xx) + Math.Abs(r.xy) + Math.Abs(r.xz) != 1 ||
        Math.Abs(r.yx) + Math.Abs(r.yy) + Math.Abs(r.yz) != 1 ||
        Math.Abs(r.zx) + Math.Abs(r.zy) + Math.Abs(r.zz) != 1)
    {
        throw new ArgumentException("Expected second argument to rotate to be an axis oriented rotation");
    }
    return new Point(a.x * r.xx + a.y * r.xy + a.z * r.xz,
        a.x * r.yx + a.y * r.yy + a.z * r.yz,
        a.x * r.zx + a.y * r.zy + a.z * r.zz);
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

public record Rotation(int xx, int xy, int xz, int yx, int yy, int yz, int zx, int zy, int zz);

public record Transformation(Rotation rot, Point translation);
