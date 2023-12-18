using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

Point up = new Point(0, -1);
Point down = new Point(0, 1);
Point left = new Point(-1, 0);
Point right = new Point(1, 0);
List<Point> directions = new List<Point>() { up, down, left, right };

HashSet<Point> edge = new HashSet<Point>();

Point pos = new Point(0, 0);
edge.Add(pos);

foreach (string line in File.ReadAllLines(fileName))
{
    string[] parts = line.Split(' ');
    Point dir = parts[0] switch
    {
        "U" => up,
        "D" => down,
        "L" => left,
        "R" => right,
        _ => throw new Exception($"Invalid direction {parts[0]}")
    };
    int steps = Int32.Parse(parts[1]);
    for (int i = 0; i < steps; i++)
    {
        pos += dir;
        edge.Add(pos);
    }
}

int minX = edge.Min(p => p.X) - 1;
int maxX = edge.Max(p => p.X) + 1;
int minY = edge.Min(p => p.Y) - 1;
int maxY = edge.Max(p => p.Y) + 1;

int gridArea = (maxX - minX + 1) * (maxY - minY + 1);

HashSet<Point> outside = new HashSet<Point>();

bool withinBounds(Point p)
{
    return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
}

IEnumerable<Point> getOutsideMoves(Point pos)
{
    return directions.Select(d => pos + d).Where(p => !edge.Contains(p) && withinBounds(p));
}

Graph<Point> outsideGraph = new GraphByFunction<Point>(getOutsideMoves);

Action<Point, Graph<Point>.VisitPath> trackOutside = (pos, path) => outside.Add(pos);

outsideGraph.BfsFrom(new Point(minX, minY), trackOutside);

Console.WriteLine($"Part 1: {gridArea - outside.Count}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}