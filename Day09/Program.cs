using Common;

var lines = File.ReadAllLines("input09.txt");

var map = lines.Zip(Enumerable.Range(0, 2000), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 2000), (c, x) => new KeyValuePair<Point, int>(new Point(x, y), int.Parse(c.ToString())))).Flatten().ToDictionary();

var directions = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

Func<Point, IEnumerable<Point>> neighbours = (pos) => directions.Select(d => d + pos);

Func<Point, IEnumerable<int>> neighbourHeights = (pos) => neighbours(pos).Select(n => map.GetOrElse(n, 10));

var lowPoints = map.Where(kv => neighbourHeights(kv.Key).All(h => h > kv.Value));
var risk = lowPoints.Select(kv => kv.Value + 1).Sum();

Console.WriteLine(risk);

Graph<Point> flowGraph = new GraphByFunction<Point>(pos => neighbours(pos).Where(n => map.ContainsKey(n) && map[n] < 9 && neighbourHeights(n).Min() == map[pos]));

var basins = lowPoints.Select(kv => new KeyValuePair<Point, int>(kv.Key, 0)).ToDictionary();

foreach (Point pos in lowPoints.Select(kv => kv.Key))
{
    flowGraph.BfsFrom(pos, (member, path) => { basins[pos]++; });
}

Console.WriteLine(basins.OrderBy(kv => -kv.Value).Take(3).Select(kv => kv.Value).Aggregate((a, b) => a * b));

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}

