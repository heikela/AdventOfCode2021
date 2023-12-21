using Common;

//string fileName = "../../../input.txt";
string fileName = "../../../testInput.txt";

var lines = File.ReadAllLines(fileName);

Dictionary<Point, char> map = new Dictionary<Point, char>();

int W = lines[0].Length;
int H = lines.Length;

for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        map[new Point(x, y)] = lines[y][x];
    }
}

Point start = map.First(kv => kv.Value == 'S').Key;
map[start] = '.';

HashSet<Point> current = new HashSet<Point>();
HashSet<Point> next = new HashSet<Point>();

Point up = new Point(0, -1);
Point down = new Point(0, 1);
Point left = new Point(-1, 0);
Point right = new Point(1, 0);

List<Point> dirs = new List<Point>() { up, down, left, right };

bool canMoveTo(Point p)
{
    int projectedX = p.X % W;
    if (projectedX < 0)
    {
        projectedX += W;
    }
    int projectedY = p.Y % H;
    if (projectedY < 0)
    {
        projectedY += H;
    }
    Point projectionToOriginal = new Point(projectedX, projectedY);
    return map[projectionToOriginal] != '#';
}

IEnumerable<Point> getMoves(Point p)
{
    foreach (var dir in dirs)
    {
        Point nextP = p + dir;
        if (canMoveTo(nextP))
        {
            yield return nextP;
        }
    }
}

/*
foreach (var p in start.getMoves())
{
    current.Add(p);
}
*/
current.Add(start);

(HashSet<Point>, HashSet<Point>, long) twoStepsFrom(HashSet<Point> liveStarts, HashSet<Point> saturated, long saturatedExcluded)
{
    HashSet<Point> current = liveStarts;
    HashSet<Point> next = new HashSet<Point>();
    for (int i = 0; i < 2; ++i)
    {
        next = new HashSet<Point>();
        foreach (var p in current)
        {
            foreach (var dir in dirs)
            {
                Point nextP = p + dir;
                if (!saturated.Contains(nextP) && !liveStarts.Contains(nextP) && canMoveTo(nextP))
                {
                    next.Add(nextP);
                }
            }
        }
        current = next;
    }
    return (current, liveStarts, saturated.LongCount() + saturatedExcluded);
}

HashSet<Point> saturated = new HashSet<Point>();
long saturatedExcluded = 0;

for (int i = 0; i < 1000; i += 2)
{
    (current, saturated, saturatedExcluded) = twoStepsFrom(current, saturated, saturatedExcluded);
}

Console.WriteLine($"Part 2: {current.LongCount() + saturated.LongCount() + saturatedExcluded}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}