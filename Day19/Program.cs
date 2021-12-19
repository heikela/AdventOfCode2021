using Common;

// See https://aka.ms/new-console-template for more information

Dictionary<int, int> RelativeDistanceHistogram(IEnumerable<int> points)
{
    Dictionary<int, int> histogram = new Dictionary<int, int>();
    List<int> pointList = points.ToList();
    for (int i = 0; i < pointList.Count - 1; i++)
    {
        for (int j = i + 1; j < pointList.Count; j++)
        {
            histogram.AddToCount(Math.Abs(pointList[j] - pointList[i]), 1);
        }
    }
    return histogram;
}

int countOverlap(Dictionary<int, int> histA, Dictionary<int, int> histB)
{
    int matchCount = 0;
    foreach (var kv in histB)
    {
        int bCount = kv.Value;
        int aCount = histA.GetOrElse(kv.Key, 0);
        matchCount += Math.Min(aCount, bCount);
    }
    return matchCount;
}

(Boolean matched, HashSet<Point> combined) CombineAreas(Scanner scannerA, Scanner scannerB)
{
    HashSet<Point> a = scannerA.Points;
    HashSet<Point> b = scannerB.Points;
    HashSet<Transformation> transformations = new HashSet<Transformation>();
    var axh = RelativeDistanceHistogram(a.Select(p => p.x));
    var bxh = RelativeDistanceHistogram(b.Select(p => p.x));
    var ayh = RelativeDistanceHistogram(a.Select(p => p.y));
    var byh = RelativeDistanceHistogram(b.Select(p => p.y));
    var azh = RelativeDistanceHistogram(a.Select(p => p.z));
    var bzh = RelativeDistanceHistogram(b.Select(p => p.z));

    int worstMatch = int.MaxValue;
    foreach (var ah in new List<Dictionary<int, int>>() { axh, ayh, azh })
    {
        int bestMatch = 0;
        foreach (var bh in new List<Dictionary<int, int>>() { bxh, byh, bzh })
        {
            bestMatch = Math.Max(bestMatch, countOverlap(ah, bh));
        }
        worstMatch = Math.Min(worstMatch, bestMatch);
    }
    Console.WriteLine(worstMatch);
    if (worstMatch >= 66)
    {
        return (true, null);
    }
    else
    {
        return (false, null);
    }

    foreach (Rotation rotation in Rotation.Rotations) {

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
                        Point transformedP = p.rotate(rotation) + translation;
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
            combined.Add(p.rotate(t.rot) + t.translation);
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

//var scanners = File.ReadLines("sampleInput19.txt").Paragraphs().Select(lines => new Scanner(lines)).ToList();
var scanners = File.ReadLines("input19.txt").Paragraphs().Select(lines => new Scanner(lines)).ToList();

Console.WriteLine(scanners.Count());

while (scanners.Count() > 1)
{
    for (int i = 0; i < scanners.Count - 1; ++i)
    {
        for (int j = i + 1; j < scanners.Count; ++j)
        {
            Scanner scannerA = scanners[i];
            Scanner scannerB = scanners[j];
            (bool combined, Scanner result) = scannerA.CombineIfMatched(scannerB);
            if (combined)
            {
                scanners.Remove(scannerA);
                scanners.Remove(scannerB);
                scanners.Add(result);
            }
        }
    }
}

Console.WriteLine($"In total there are {scanners.Single().Points.Count()} beacons");

public record Point(int x, int y, int z)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y, a.z - b.z);

    public Point rotate(Rotation r)
    {
        if (Math.Abs(r.xx) + Math.Abs(r.xy) + Math.Abs(r.xz) != 1 ||
            Math.Abs(r.yx) + Math.Abs(r.yy) + Math.Abs(r.yz) != 1 ||
            Math.Abs(r.zx) + Math.Abs(r.zy) + Math.Abs(r.zz) != 1)
        {
            throw new ArgumentException("Expected second argument to rotate to be an axis oriented rotation");
        }
        return new Point(
            this.x * r.xx + this.y * r.xy + this.z * r.xz,
            this.x * r.yx + this.y * r.yy + this.z * r.yz,
            this.x * r.zx + this.y * r.zy + this.z * r.zz);
    }

}

public record AxisTransformation(int rotate, int translate)
{
    public int Apply(int x) {
        return this.rotate * x + this.translate;
    }
}

public record Rotation(int xx, int xy, int xz, int yx, int yy, int yz, int zx, int zy, int zz)
{
    public static List<Rotation> Rotations = new List<Rotation>()
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



        new Rotation(-1, 0, 0, 0, 1, 0, 0, 0, 1),
        new Rotation(-1, 0, 0, 0, 0, 1, 0, -1, 0),
        new Rotation(-1, 0, 0, 0, -1, 0, 0, 0, -1),
        new Rotation(-1, 0, 0, 0, 0, -1, 0, 1, 0),

        new Rotation(1, 0, 0, 0, 1, 0, 0, 0, -1),
        new Rotation(1, 0, 0, 0, 0, 1, 0, 1, 0),
        new Rotation(1, 0, 0, 0, -1, 0, 0, 0, 1),
        new Rotation(1, 0, 0, 0, 0, -1, 0, -1, 0),

        new Rotation(0, 0, -1, 0, 1, 0, -1, 0, 0),
        new Rotation(0, 0, -1, 1, 0, 0, 0, 1, 0),
        new Rotation(0, 0, -1, 0, -1, 0, 1, 0, 0),
        new Rotation(0, 0, -1, -1, 0, 0, 0, -1, 0),

        new Rotation(0, 0, 1, 0, 1, 0, 1, 0, 0),
        new Rotation(0, 0, 1, 1, 0, 0, 0, -1, 0),
        new Rotation(0, 0, 1, 0, -1, 0, -1, 0, 0),
        new Rotation(0, 0, 1, -1, 0, 0, 0, 1, 0),

        new Rotation(0, -1, 0, -1, 0, 0, 0, 0, -1),
        new Rotation(0, -1, 0, 0, 0, -1, 1, 0, 0),
        new Rotation(0, -1, 0, 1, 0, 0, 0, 0, 1),
        new Rotation(0, -1, 0, 0, 0, 1, -1, 0, 0),

        new Rotation(0, 1, 0, -1, 0, 0, 0, 0, 1),
        new Rotation(0, 1, 0, 0, 0, -1, -1, 0, 0),
        new Rotation(0, 1, 0, 1, 0, 0, 0, 0, -1),
        new Rotation(0, 1, 0, 0, 0, 1, 1, 0, 0),

    };

}

public record Transformation(Rotation rot, Point translation);

public class Scanner
{
    public HashSet<Point> Points;
    public Dictionary<Point, List<(Point a, Point b)>> PointsByRelativeDistance;

    public Scanner(IEnumerable<string> lines)
    {
        Points = new HashSet<Point>();
        foreach (string line in lines.Skip(1))
        {
            int[] parts = line.Split(",").Select(int.Parse).ToArray();
            Points.Add(new Point(parts[0], parts[1], parts[2]));
        }
        OrganiseByRelativeDistances();
    }


    public static Dictionary<Point, List<(Point a, Point b)>> RelativeDistances(List<Point> points)
    {
        var result = new Dictionary<Point, List<(Point a, Point b)>>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                Point a = points[i];
                Point b = points[j];

                Point dist1 = b - a;
                if (!result.ContainsKey(dist1))
                {
                    result.Add(dist1, new List<(Point a, Point b)>());
                }
                result[dist1].Add((a, b));

                Point dist2 = a - b;
                if (!result.ContainsKey(dist2))
                {
                    result.Add(dist2, new List<(Point a, Point b)>());
                }
                result[dist2].Add((b, a));
            }
        }
        return result;
    }

    private void OrganiseByRelativeDistances()
    {
        PointsByRelativeDistance = RelativeDistances(Points.ToList());
    }

    private Dictionary<Point, List<(Point a, Point b)>> RotatedRelativeDistances(Rotation rot)
    {
        return RelativeDistances(Points.Select(p => p.rotate(rot)).ToList());
    }

    public bool FindMatch(Scanner other)
    {
        int bestMatch = 0;
        foreach (Rotation rot in Rotation.Rotations)
        {
            var rotDist = other.RotatedRelativeDistances(rot);
            var distanceMatches = rotDist.Keys.Intersect(PointsByRelativeDistance.Keys);
            bestMatch = Math.Max(bestMatch, distanceMatches.Count());
        }
//        Console.WriteLine(bestMatch);
        return bestMatch >= 66;
    }
    public (bool, Scanner) CombineIfMatched(Scanner other)
    {
        Rotation goodRotation = null;
        Point goodTranslation = null;
        int originalPoints = this.Points.Count();
        foreach (Rotation rot in Rotation.Rotations)
        {
            var rotDist = other.RotatedRelativeDistances(rot);
            var distanceMatches = rotDist.Keys.Intersect(PointsByRelativeDistance.Keys);
            if (distanceMatches.Count() >= 66)
            {
                Dictionary<Point, int> possibleTranslations = new Dictionary<Point, int>();
                foreach (var dist in distanceMatches)
                {
                    for (int i = 0; i < PointsByRelativeDistance[dist].Count; i++)
                    {
                        for (int j = 0; j < rotDist[dist].Count; j++)
                        {
                            possibleTranslations.AddToCount(PointsByRelativeDistance[dist][i].a - rotDist[dist][j].a, 1);
                        }
                    }
                }
                var translations = possibleTranslations.Where(kv => kv.Value >= 132);
                foreach (var translation in translations)
                {
                    Console.WriteLine($"Possible match with rotation {rot}, translation {translation.Key} with {translation.Value} vectors matching");
                    if (goodTranslation != null)
                    {
                        throw new Exception("Found multiple different potentially good matches");
                    }
                    goodTranslation = translation.Key;
                    goodRotation = rot;
                }
            }
        }
        if (goodTranslation == null)
        {
            return (false, null);
        }
        else
        {
            foreach (var point in other.Points)
            {
                Points.Add(point.rotate(goodRotation) + goodTranslation);
            }
            OrganiseByRelativeDistances();
        }
        return (true, this);
    }
}

