using Common;

var lines = File.ReadAllLines("input11.txt");

var octopi = lines.Zip(Enumerable.Range(0, 10), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 10), (c, x) => new KeyValuePair<Point, int>(new Point(x, y), int.Parse(c.ToString())))).Flatten().ToDictionary();

var directions = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(-1, -1), new Point(1, 1), new Point(1, -1), new Point(-1, 1) };

Func<Point, IEnumerable<Point>> neighbours = (pos) => directions.Select(d => d + pos);

int flashes = 0;
int time = 0;
for (time = 0; flashes != 100; time++)
{
    flashes = 0;
    var newOctopi = octopi.Select(kv => new KeyValuePair<Point, int>(kv.Key, kv.Value + 1)).ToDictionary();
    bool found = false;
    do
    {
        found = false;
        var flashing = newOctopi.SkipWhile(kv => kv.Value <= 9);
        if (flashing.Any())
        {
            var firstFlashing = flashing.First();
            newOctopi[firstFlashing.Key] = 0;
            flashes++;
            found = true;
            foreach (var pos in neighbours(firstFlashing.Key))
            {
                if (newOctopi.ContainsKey(pos) && newOctopi[pos] > 0) newOctopi[pos]++;
            }
        }
    } while (found);
    octopi = newOctopi;
}

Console.WriteLine(time);

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}


