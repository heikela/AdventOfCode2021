using Common;


var inputFile = "sampleInput20.txt";
//var inputFile = "input20.txt";

var lines = File.ReadLines(inputFile).ToList();

var enhancer = lines[0].ToList();

var grid = lines.Skip(2).Zip(Enumerable.Range(0, 1000), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 1000), (c, x) => new KeyValuePair<Point, char>(new Point(x, y), c == '#' ? '1' : '0'))).Flatten().ToDictionary();

var directions = new[] { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1) };

IEnumerable<Point> Neighbours(Point pos)
{
    return directions.Select(d => d + pos);
}

Dictionary<Point, char> Enhance(Dictionary<Point, char> image)
{
    int xMin = image.Keys.Min(p => p.x) - 1;
    int xMax = image.Keys.Max(p => p.x) + 1;
    int yMin = image.Keys.Min(p => p.y) - 1;
    int yMax = image.Keys.Max(p => p.y) + 1;

    var result = new Dictionary<Point, char>();

    for (int x = xMin; x <= xMax; ++x)
    {
        for (int y = yMin; y <= yMax; ++y)
        {
            Point pos = new Point(x, y);
            var neighbours = Neighbours(pos);
            var neighbourValues = neighbours.Select(p => image.GetOrElse(p, '0')).ToArray();
            int enhancerIndex = Convert.ToInt32(new String(neighbourValues), 2);
            char value = enhancer[enhancerIndex] == '#' ? '1' : '0';
            result.Add(pos, value);
        }
    }
    return result;
}

Console.WriteLine($"Twice enhanced, we have {Enhance(Enhance(grid)).Count(kv => kv.Value == '1')} lit pixels");

// not 4924

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}


