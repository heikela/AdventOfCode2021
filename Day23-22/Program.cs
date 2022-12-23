//var input = File.ReadAllLines("../../../testInput2.txt");
var input = File.ReadAllLines("../../../input.txt");

Point N = new Point(0, -1);
Point S = new Point(0, 1);
Point W = new Point(-1, 0);
Point E = new Point(1, 0);

Queue<DirAndChecks> directions = new Queue<DirAndChecks>();

directions.Enqueue(new DirAndChecks(N, new List<Point>() { N + E, N, N + W }));
directions.Enqueue(new DirAndChecks(S, new List<Point>() { S + E, S, S + W }));
directions.Enqueue(new DirAndChecks(W, new List<Point>() { N + W, W, S + W }));
directions.Enqueue(new DirAndChecks(E, new List<Point>() { N + E, E, S + E }));

IEnumerable<Point> Neighbours(Point p)
{
    yield return p + N;
    yield return p + N + E;
    yield return p + N + W;
    yield return p + E;
    yield return p + W;
    yield return p + S;
    yield return p + S + E;
    yield return p + S + W;
}

HashSet<Point> elves = new HashSet<Point>();

int y = 0;
foreach (var line in input)
{
    int x = 0;
    foreach (char c in line)
    {
        if (c == '#')
        {
            elves.Add(new Point(x, y));
        }
        ++x;
    }
    ++y;
}

for (int t = 0; t < int.MaxValue; t++)
{
    Dictionary<Point, List<Point>> movesFrom = new Dictionary<Point, List<Point>>();
    foreach (Point p in elves)
    {
        if (Neighbours(p).Any(x => elves.Contains(x)))
        {
            foreach (DirAndChecks dir in directions)
            {
                if (dir.toCheck.All(check => !elves.Contains(p + check)))
                {
                    Point proposed = dir.dir + p;
                    if (!movesFrom.ContainsKey(proposed))
                    {
                        movesFrom.Add(proposed, new List<Point>());
                    }
                    movesFrom[proposed].Add(p);
                    break;
                }
            }
        }
    }
    foreach (KeyValuePair<Point, List<Point>> kv in movesFrom)
    {
        if (kv.Value.Count == 1)
        {
            elves.Remove(kv.Value[0]);
            elves.Add(kv.Key);
        }
    }
    DirAndChecks first = directions.Dequeue();
    directions.Enqueue(first);

    /*
    Console.WriteLine("--------------------------");
    VisualizeMap(elves);
    Console.WriteLine();
    */

    if (movesFrom.Count == 0)
    {
        Console.WriteLine(t + 1);
        break;
    }
}

void VisualizeMap(HashSet<Point> elves)
{
    int minX = elves.Select(p => p.x).Min();
    int maxX = elves.Select(p => p.x).Max();
    int minY = elves.Select(p => p.y).Min();
    int maxY = elves.Select(p => p.y).Max();
    for (int y = minY; y <= maxY; y++)
    {
        for (int x = minX; x <= maxX; x++)
        {
            Console.Write(elves.Contains(new Point(x, y)) ? '#' : '.');
        }
        Console.WriteLine();
    }
}


int minX = elves.Select(p => p.x).Min();
int maxX = elves.Select(p => p.x).Max();
int minY = elves.Select(p => p.y).Min();
int maxY = elves.Select(p => p.y).Max();

Console.WriteLine((maxX - minX + 1) * (maxY - minY + 1) - elves.Count());

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}

public record DirAndChecks(Point dir, List<Point> toCheck);
