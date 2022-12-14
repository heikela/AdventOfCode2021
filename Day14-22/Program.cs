using Common;

//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

Dictionary<Point, char> Cave = new Dictionary<Point, char>();
Point SandSource = new Point(500, 0);
Point Down = new Point(0, 1);
List<Point> Directions = new List<Point>()
{
    Down,
    new Point(-1, 1),
    new Point(1, 1)
};

foreach (var line in input)
{
    var points = line.Split(" -> ");
    var xy = points[0].Split(',');
    Point start = new Point(int.Parse(xy[0]), int.Parse(xy[1]));
    Point current = start;
    Cave.AddOrSet(current, '#');
    foreach (string nextPointDesc in points.Skip(1))
    {
        xy = nextPointDesc.Split(',');
        Point next = new Point(int.Parse(xy[0]), int.Parse(xy[1]));
        Point dir = Normalize(next - current);

        do
        {
            current += dir;
            Cave.AddOrSet(current, '#');
        } while (current != next);
    }
}

int bottom = Cave.Keys.Select(p => p.y).Max();

Dictionary<Point, char> Cave2 = new Dictionary<Point, char>(Cave);

for (int x = SandSource.x - bottom - 4; x < SandSource.x + bottom + 4; x++)
{
    Cave2.Add(new Point(x, bottom + 2), '#');
}

int sand = 0;
Point currentSand = SandSource;

while (currentSand.y < bottom)
{
    bool sandStopped = true;
    foreach (var dir in Directions)
    {
        if (!Cave.ContainsKey(currentSand + dir))
        {
            currentSand += dir;
            sandStopped = false;
            break;
        }
    }
    if (sandStopped)
    {
        if (currentSand == SandSource)
        {
            throw new Exception("Did not expect sand source to get blocked");
        }
        Cave.Add(currentSand, 'o');
        sand++;
        currentSand = SandSource;
    }
}

/*
for (int y = Cave.Keys.Select(p => p.y).Min(); y <= Cave.Keys.Select(p => p.y).Max(); ++y)
{
    for (int x = Cave.Keys.Select(p => p.x).Min(); x <= Cave.Keys.Select(p => p.x).Max(); ++x)
    {
        Console.Write(Cave.GetOrElse(new Point(x, y), '.'));
    }
    Console.WriteLine();
}
*/
Console.WriteLine(sand);

// Part 2

sand = 0;
currentSand = SandSource;

while (!Cave2.ContainsKey(SandSource))
{
    bool sandStopped = true;
    foreach (var dir in Directions)
    {
        if (!Cave2.ContainsKey(currentSand + dir))
        {
            currentSand += dir;
            sandStopped = false;
            break;
        }
    }
    if (sandStopped)
    {
        Cave2.Add(currentSand, 'o');
        sand++;
        currentSand = SandSource;
    }
}

Console.WriteLine(sand);

Point Normalize(Point p)
{
    return new Point(Math.Max(Math.Min(p.x, 1), -1), Math.Max(Math.Min(p.y, 1), -1));
}

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}
