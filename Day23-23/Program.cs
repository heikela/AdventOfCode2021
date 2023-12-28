using Common;

var lines = new AoCUtil().GetInput(2023, 23);
//var lines = new AoCUtil().GetTestBlock(2023, 23, 0);

Point up = new Point(0, -1);
Point down = new Point(0, 1);
Point left = new Point(-1, 0);
Point right = new Point(1, 0);

Point[] directions = new Point[] { up, down, left, right };

Dictionary<Point, char> map = new Dictionary<Point, char>();

for (int y = 0; y < lines.Length; ++y)
{
    string line = lines[y];
    for (int x = 0; x < line.Length; ++x)
    {
        map.Add(new Point(x, y), line[x]);
    }
}

Point start = map.Where(kv => kv.Key.Y == 0 && kv.Value == '.').Single().Key;
Point end = map.Where(kv => kv.Key.Y == lines.Length - 1 && kv.Value == '.').Single().Key;

IEnumerable<Point> GetNext1(Point p)
{
    char current = map[p];
    if (current == '.')
    {
        foreach (var dir in directions)
        {
            Point next = p + dir;
            if (map.GetOrElse(next, '#') != '#')
            {
                yield return next;
            }
        }
    }
    else if (current == '<')
    {
        yield return p + left;
    }
    else if (current == '>')
    {
        yield return p + right;
    }
    else if (current == '^')
    {
        yield return p + up;
    }
    else if (current == 'v')
    {
        yield return p + down;
    }
}

IEnumerable<Point> GetNext2(Point p)
{
    char current = map[p];
    foreach (var dir in directions)
    {
        Point next = p + dir;
        if (map.GetOrElse(next, '#') != '#')
        {
            yield return next;
        }
    }
}

int longest1 = 0;
void explore1(Point current, HashSet<Point> visited, int length)
{
    if (current == end)
    {
        longest1 = Math.Max(longest1, length);
        return;
    }
    HashSet<Point> newVisited = new HashSet<Point>(visited);
    newVisited.Add(current);
    foreach (var next in GetNext1(current))
    {
        if (visited.Contains(next))
        {
            continue;
        }
        explore1(next, newVisited, length + 1);
    }
}

//explore1(start, new HashSet<Point>(), 0);

Dictionary<Point, List<(Point, int)>> nextJunctions = new Dictionary<Point, List<(Point, int)>>();
HashSet<Point> doneDirections = new HashSet<Point>();

void addPath(Point start, Point end, int length)
{
    if (!nextJunctions.ContainsKey(start))
    {
        nextJunctions[start] = new List<(Point, int)>();
    }
    nextJunctions[start].Add((end, length));
    if (!nextJunctions.ContainsKey(end))
    {
        nextJunctions[end] = new List<(Point, int)>();
    }
    nextJunctions[end].Add((start, length));
}


void createPart2Map(Point current, Point prev)
{
    Point pathStart = prev;
    int length = 1;
    while (true)
    {
        List<Point> connectedPoints = GetNext2(current).ToList();
        if (connectedPoints.Count > 2 || current == end)
        {
            addPath(pathStart, current, length);
            doneDirections.Add(prev);
            foreach (Point newStart in connectedPoints.Where(p => !doneDirections.Contains(p)))
            {
                doneDirections.Add(newStart);
                createPart2Map(newStart, current);
            }
            break;
        }
        if (connectedPoints.Count <= 1)
        {
            break;
        }
        Point next = connectedPoints.Single(p => p != prev);
        prev = current;
        current = next;
        length++;
    }
}

int longest2 = 0;
void explore2(Point current, HashSet<Point> visited, int length)
{
    if (current == end)
    {
        longest2 = Math.Max(longest2, length);
        return;
    }
    HashSet<Point> newVisited = new HashSet<Point>(visited);
    newVisited.Add(current);
    foreach (var (next, dist) in nextJunctions[current])
    {
        if (visited.Contains(next))
        {
            continue;
        }
        explore2(next, newVisited, length + dist);
    }
}

createPart2Map(start + down, start);
explore2(start, new HashSet<Point>(), 0);

Console.WriteLine($"Part 1: {longest1}");
Console.WriteLine($"Part 2: {longest2}");
// not 6322

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}
