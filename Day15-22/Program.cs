var input = File.ReadAllLines("../../../input.txt");
//var input = File.ReadAllLines("../../../testInput.txt");

//int relevantLine = 10;
int relevantLine = 2000000;

List<Span> spansOnLine = new List<Span>();

HashSet<Point> beacons = new HashSet<Point>();

Dictionary<Point, int> exclusions = new Dictionary<Point, int>();

foreach (string line in input)
{
    string[] parts = line.Split(' ');
    Point sensor = new Point(int.Parse(parts[2].Substring(2, parts[2].Length - 3)), int.Parse(parts[3].Substring(2, parts[3].Length - 3)));
    Point beacon = new Point(int.Parse(parts[8].Substring(2, parts[8].Length - 3)), int.Parse(parts[9].Substring(2)));

    beacons.Add(beacon);

    int dist = sensor.ManhattanDist(beacon);

    exclusions.Add(sensor, dist);

    int spanStart = sensor.x - dist + Math.Abs(sensor.y - relevantLine);
    int spanEnd = sensor.x + dist - Math.Abs(sensor.y - relevantLine);
    
    if (spanStart <= spanEnd)
    {
        spansOnLine.Add(new Span(spanStart, spanEnd));
    }
}

HashSet<int> covered = new HashSet<int>();

foreach (Span span in spansOnLine)
{
    for (int x = span.first; x <= span.last; x++)
    {
        covered.Add(x);
    }
}

foreach (Point beacon in beacons)
{
    if (beacon.y == relevantLine)
    {
        if (covered.Contains(beacon.x))
        {
            covered.Remove(beacon.x);
        }
    }
}

Console.WriteLine(covered.Count);

//int searchSize = 20;
int searchSize = 4000000;

for (int y = 0; y <= searchSize; ++y)
{
    if (y % 100000 == 0)
    {
        Console.Write('.');
    }
    for (int x = 0; x <= searchSize;)
    {
        int nextX = x;
        foreach (var beacon in exclusions)
        {
            if (beacon.Key.x - beacon.Value + Math.Abs(beacon.Key.y - y) <= x)
            {
                int pushedX = beacon.Key.x + beacon.Value - Math.Abs(beacon.Key.y - y) + 1;
                if (pushedX > nextX)
                {
                    nextX = pushedX;
                }
            }
        }
        if (nextX == x)
        {
            Console.WriteLine();
            long tuning = x;
            tuning = tuning * 4000000;
            tuning = tuning + y;
            Console.WriteLine(tuning);
            goto Done;
        }
        x = nextX;
    }
}
Done:
Console.WriteLine();

public record Span(int first, int last)
{
    public bool Overlaps(Span other)
    {
        if (this.last >= other.first && this.first <= other.last)
        {
            return true;
        }
        if (other.last >= this.first && other.first <= this.last)
        {
            return true;
        }
        return false;
    }

    public Span Combine(Span other)
    {
        if (!Overlaps(other))
        {
            throw new ArgumentException("Cannot merge with non-overlapping span");
        }
        return new Span(Math.Min(first, other.first), Math.Max(last, other.last));
    }
}

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);

    public int ManhattanDist(Point other)
    {
        Point d = other - this;
        return Math.Abs(d.x) + Math.Abs(d.y);
    }
}

