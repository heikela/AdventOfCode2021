var input = File.ReadAllLines("../../../input.txt");
//var input = File.ReadAllLines("../../../testInput.txt");

List<Point> directions = new List<Point>()
{
    new Point(1, 0, 0),
    new Point(-1, 0, 0),
    new Point(0, 1, 0),
    new Point(0, -1, 0),
    new Point(0, 0, 1),
    new Point(0, 0, -1)
};

HashSet<Point> lava = new HashSet<Point>();

foreach (var line in input)
{
    string[] parts = line.Split(',');
    lava.Add(new Point(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
}

int allSurface = 0;

foreach (var point in lava)
{
    foreach (var d in directions)
    {
        if (!lava.Contains(point + d))
        {
            allSurface++;
        }
    }
}
// See https://aka.ms/new-console-template for more information
Console.WriteLine($"lava surface is {allSurface}");

int minX = lava.Min(p => p.x) - 1;
int maxX = lava.Max(p => p.x) + 1;
int minY = lava.Min(p => p.y) - 1;
int maxY = lava.Max(p => p.y) + 1;
int minZ = lava.Min(p => p.z) - 1;
int maxZ = lava.Max(p => p.z) + 1;

bool InBounds(Point p)
{
    if (p.x < minX || p.x > maxX)
    {
        return false;
    }
    if (p.y < minY || p.y > maxY)
    {
        return false;
    }
    if (p.z < minZ || p.z > maxZ)
    {
        return false;
    }
    return true;
}

int surface = 0;

HashSet<Point> frontier = new HashSet<Point>() { new Point(minX, minY, minZ)};
HashSet<Point> visited = new HashSet<Point>();

while (frontier.Count > 0)
{
    Point current = frontier.First();
    frontier.Remove(current);
    visited.Add(current);
    foreach (Point d in directions)
    {
        Point neighbour = current + d;
        if (!InBounds(neighbour))
        {
            continue;
        }
        if (lava.Contains(neighbour))
        {
            surface++;
        }
        else if (!visited.Contains(neighbour))
        {
            frontier.Add(neighbour);
        }
    }
}

Console.WriteLine($"cooling lava surface is {surface}");

public record Point(int x, int y, int z)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.z + b.z);
}

