using Common;

var lines = File.ReadAllLines("input11.txt");

var octopi = lines.Zip(Enumerable.Range(0, 10), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 10), (c, x) => new KeyValuePair<Point, int>(new Point(x, y), int.Parse(c.ToString())))).Flatten().ToDictionary();

var directions = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(-1, -1), new Point(1, 1), new Point(1, -1), new Point(-1, 1) };

Func<Point, IEnumerable<Point>> neighbours = (pos) => directions.Select(d => d + pos);

int flashes = 0;
int time = 0;
int flashesThisTime = 0;
while (flashesThisTime != 100 || time < 100)
{
    flashesThisTime = 0;
    octopi = octopi.Select(kv => new KeyValuePair<Point, int>(kv.Key, kv.Value + 1)).ToDictionary();
    var flashingPositions = octopi.Where(kv => kv.Value > 9).Select(kv => kv.Key);
    while (flashingPositions.Any())
    {
        foreach (var flashPos in flashingPositions)
        {
            octopi[flashPos] = 0;
            flashesThisTime++;
            foreach (var pos in neighbours(flashPos))
            {
                if (octopi.ContainsKey(pos) && octopi[pos] > 0) octopi[pos]++;
            }
        }
        flashingPositions = octopi.Where(kv => kv.Value > 9).Select(kv => kv.Key);
    }
    time++;
    flashes += flashesThisTime;
    if (time == 100)
    {
        Console.WriteLine($"There are {flashes} individual flashes in the first 100 time steps");
    }
    if (flashesThisTime == 100)
    {
        Console.WriteLine($"All octopi flash together at time {time}");
    }
}

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}


