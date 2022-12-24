using Common;

//var input = File.ReadAllLines("../../../testInput2.txt");
var input = File.ReadAllLines("../../../input.txt");

Point N = new Point(0, -1, 1);
Point S = new Point(0, 1, 1);
Point W = new Point(-1, 0, 1);
Point E = new Point(1, 0, 1);
Point stay = new Point(0, 0, 1);

Point start = new Point(0, -1, 0);

Console.WriteLine(input.Count());

Dictionary<int, List<Blizzard>> rowBlizzards = new Dictionary<int, List<Blizzard>>();
Dictionary<int, List<Blizzard>> colBlizzards = new Dictionary<int, List<Blizzard>>();

int yLimit = input.Count() - 2;
int xLimit = input.First().Count() - 2;

List<Point> directions = new List<Point>() {
    N, S, W, E, stay
};

int y = -1;
foreach (var line in input)
{
    int x = -1;
    rowBlizzards[y] = new List<Blizzard>();
    foreach (char c in line)
    {
        if (y == 0)
        {
            colBlizzards[x] = new List<Blizzard>();
        }
        if (c == '>')
        {
            rowBlizzards[y].Add(new Blizzard(x, 1, xLimit));
        }
        if (c == '<')
        {
            rowBlizzards[y].Add(new Blizzard(x, -1, xLimit));
        }
        if (c == 'v')
        {
            colBlizzards[x].Add(new Blizzard(y, 1, yLimit));
        }
        if (c == '^')
        {
            colBlizzards[x].Add(new Blizzard(y, -1, yLimit));
        }
        ++x;
    }
    ++y;
}

GraphByFunction<Point> moveGraph = new GraphByFunction<Point>(ValidMovesFrom);

// We can do this in three segments because the start and exit squares are safe for waiting

var path1 = moveGraph.ShortestPathTo(start, IsExit);
Point exit1 = path1.GetNodesOnPath().Last();
var path2 = moveGraph.ShortestPathTo(exit1, IsStart);
Point start2 = path2.GetNodesOnPath().Last();
var path3 = moveGraph.ShortestPathTo(start2, IsExit);
Point exit2 = path3.GetNodesOnPath().Last();


Console.WriteLine($"Shortest path to exit takes {path1.GetLength()} steps");
Console.WriteLine($"Shortest path to exit takes {exit1.t} steps");
Console.WriteLine($"Shortest path to exit, back to start, and again to exit takes {exit2.t} steps");

// not 562

IEnumerable<Point> ValidMovesFrom(Point p)
{
    foreach (Point dir in directions)
    {
        Point candidate = p + dir;
        if (candidate.x < 0 || candidate.x >= xLimit)
        {
            continue;
        }
        if ((candidate.y < 0 && !IsStart(candidate)) || (candidate.y >= yLimit && !IsExit(candidate)))
        {
            continue;
        }
        if (rowBlizzards[candidate.y].Any(b => b.posAtTime(candidate.t) == candidate.x))
        {
            continue;
        }
        if (colBlizzards[candidate.x].Any(b => b.posAtTime(candidate.t) == candidate.y))
        {
            continue;
        }
        yield return candidate;
    }
}

bool IsExit(Point p)
{
    return p.x == xLimit - 1 && p.y == yLimit;
}

bool IsStart(Point p)
{
    return p.x == 0 && p.y == -1;
}

public record Point(int x, int y, int t)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.t + b.t);
}

public record Blizzard(int startPos, int dir, int limit)
{
    public int posAtTime(int t)
    {
        return (((startPos + t * dir) % limit) + limit) % limit;
    }
}
