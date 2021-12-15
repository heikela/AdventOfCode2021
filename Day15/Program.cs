using Common;
using Priority_Queue;

var risk = File.ReadAllLines("input15.txt").Zip(Enumerable.Range(0, 1000), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 1000), (c, x) => new KeyValuePair<Point, int>(new Point(x, y), int.Parse(c.ToString())))).Flatten().ToDictionary();

var directions = new List<Point>() { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

IEnumerable<(Point node, int edgeWeight)> GetNeighbours(Dictionary<Point, int> grid, Point pos)
{
    foreach (var dir in directions)
    {
        var npos = pos + dir;
        if (grid.ContainsKey(npos))
        {
            yield return (npos, grid[npos]);
        }
    }
    yield break;
}

long DijkstraFrom(Dictionary<Point, int> grid, Point start, Point end)
{
    SimplePriorityQueue<Point, int> queue = new SimplePriorityQueue<Point, int>();
    Dictionary<Point, int> bestSoFar = new Dictionary<Point, int>();
    Dictionary<Point, Point> predecessor = new Dictionary<Point, Point>();

    queue.Enqueue(start, 0);
    bestSoFar.Add(start, 0);
    predecessor.Add(start, start);
    while (queue.Count != 0)
    {
        Point node = queue.Dequeue();
        if (node == end)
        {
            return bestSoFar[node];
        }
        foreach ((Point node, int edgeWeight) neighbour in GetNeighbours(grid, node))
        {
            int potentialDist = bestSoFar[node] + neighbour.edgeWeight;
            if (potentialDist < bestSoFar.GetOrElse(neighbour.node, int.MaxValue))
            {
                bestSoFar.AddOrSet(neighbour.node, potentialDist);
                predecessor.AddOrSet(neighbour.node, node);
                if (queue.Contains(neighbour.node))
                {
                    queue.UpdatePriority(neighbour.node, potentialDist);
                }
                else
                {
                    queue.Enqueue(neighbour.node, potentialDist);
                }
            }
        }
    }
    return -1;
}

int maxX = risk.Keys.Max(p => p.x);
int maxY = risk.Keys.Max(p => p.y);

Console.WriteLine($"{ DijkstraFrom(risk, new Point(0, 0), new Point(maxX, maxY))}");

var risk2 = new Dictionary<Point, int>();

var gridSize = risk.Keys.Max(p => p.x) + 1;

for (int y = 0; y < 5; ++y)
{
    Point translate = new Point(0, y * gridSize);

    for (int x = 0; x < 5; ++x)
    {
        foreach (var kv in risk)
        {
            int newValue = kv.Value + x + y;
            while (newValue > 9)
            {
                newValue = newValue - 9;
            }
            risk2.Add(kv.Key + translate, newValue);
        }
        translate = translate + new Point(gridSize, 0);
    }
}

maxX = risk2.Keys.Max(p => p.x);
maxY = risk2.Keys.Max(p => p.y);

Console.WriteLine($"{ DijkstraFrom(risk2, new Point(0, 0), new Point(maxX, maxY))}");

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}


