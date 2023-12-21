using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

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
    return map.GetOrElse(p, '#') != '#';
}

bool canMoveToInf(Point p)
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

IEnumerable<Point> getMovesInf(Point p)
{
    foreach (var dir in dirs)
    {
        Point nextP = p + dir;
        if (canMoveToInf(nextP))
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
            foreach (var nextP in getMovesInf(p))
            {
                if (!saturated.Contains(nextP) && !liveStarts.Contains(nextP))
                {
                    next.Add(nextP);
                }
            }
        }
        current = next;
    }
    return (current, liveStarts, saturated.LongCount() + saturatedExcluded);
}

Point mapIndex(Point p)
{
    int xResult = p.X / W;
    int yResult = p.Y / H;
    if (p.X < 0)
    {
        xResult = (p.X + 1) / W - 1;
    }
    if (p.Y < 0)
    {
        yResult = (p.Y + 1) / H - 1;
    }
    return new Point(xResult, yResult);
}

List<Point> mapsToCheck = dirs.ToList();
mapsToCheck.Add(new Point(0, 0));
mapsToCheck.Add(up + left);
mapsToCheck.Add(up + right);
mapsToCheck.Add(down + left);
mapsToCheck.Add(down + right);
mapsToCheck.Add(down + down);
mapsToCheck.Add(right + right);
mapsToCheck.Add(left + left);
mapsToCheck.Add(up + up);

int toVisitCount = (map.Where(kv => kv.Value == '.').Select(kv => kv.Key).Count() - 2) * mapsToCheck.Count;
HashSet<Point> visited = new HashSet<Point>();
Dictionary<Point, int> dist = new Dictionary<Point, int>();
Dictionary<Point, int> distDiff = new Dictionary<Point, int>();
int step = 0;
while (visited.Count != toVisitCount)
{
    current = current.SelectMany(p => getMovesInf(p)).ToHashSet();
    step++;
    foreach (var p in current)
    {
        if (!visited.Contains(p) && mapsToCheck.Contains(mapIndex(p)))
        {
            visited.Add(p);
            dist[p] = step;
            distDiff[p] = step - start.ManhattanDistance(p);
        }
    }
}

foreach (var g in distDiff.GroupBy(kv => kv.Value))
{
    Console.WriteLine($"Distance difference {g.Key}: {g.Count()}");
}

HashSet<Point> saturated = new HashSet<Point>();
long saturatedExcluded = 0;
/*
for (int i = 0; i < 1000; i += 2)
{
    (current, saturated, saturatedExcluded) = twoStepsFrom(current, saturated, saturatedExcluded);
}
*/

Console.WriteLine($"Part 2: {current.LongCount() + saturated.LongCount() + saturatedExcluded}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

    public int ManhattanDistance(Point other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }
}