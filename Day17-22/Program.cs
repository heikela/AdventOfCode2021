//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

List<Point> windForecast = input.First().Select(c => c == '<' ? new Point(-1, 0) : new Point(1, 0)).ToList();

Point down = new Point(0, -1);

List<List<Point>> rocks = new List<List<Point>>()
{
    new List<Point>()
    {
        new Point(0, 0),
        new Point(1, 0),
        new Point(2, 0),
        new Point(3, 0)
    },
    new List<Point>()
    {
        new Point(0, 1),
        new Point(1, 1),
        new Point(2, 1),
        new Point(1, 0),
        new Point(1, 2)
    },
    new List<Point>()
    {
        new Point(0, 0),
        new Point(1, 0),
        new Point(2, 0),
        new Point(2, 1),
        new Point(2, 2),
    },
    new List<Point>()
    {
        new Point(0, 0),
        new Point(0, 1),
        new Point(0, 2),
        new Point(0, 3)
    },
    new List<Point>()
    {
        new Point(0, 0),
        new Point(1, 0),
        new Point(0, 1),
        new Point(1, 1)
    },
};

HashSet<Point> oldRocks = new HashSet<Point>();
/*List<long> surface = new List<long>()
{
    0, 0, 0, 0, 0, 0, 0
};*/

long maxHeight = 0;
int nextStoneIndex = 0;
int nextWindIndex = 0;

List<Point> GetNextStone()
{
    List<Point> shape = rocks[nextStoneIndex];
    nextStoneIndex++;
    nextStoneIndex = nextStoneIndex % rocks.Count;
    long y = 4;
    Point translation = new Point(3, maxHeight + 4);
    return shape.Select(p => p + translation).ToList();
}

Point GetNextWind()
{
    Point result = windForecast[nextWindIndex];
    nextWindIndex++;
    nextWindIndex = nextWindIndex % windForecast.Count;
    return result;
}

bool Collides(IEnumerable<Point> points)
{
    if (points.Any(p => p.y <= 0)) {
        return true;
    }
    if (points.Any(p => p.x <= 0)) {
        return true;
    }
    if (points.Any(p => p.x >= 8)) {
        return true;
    }
    if (points.Any(p => oldRocks.Contains(p))) {
        return true;
    }
    return false;
}

void ShowRocks()
{
    for (long y = oldRocks.Max(p => p.y); y >= 0; y--)
    {
        for (int x = 0; x < 9; x++)
        {
            if (oldRocks.Contains(new Point(x, y)))
            {
                Console.Write('#');
            }
            else
            {
                if (y == 0)
                {
                    Console.Write('-');
                }
                else if (x == 0 || x == 8)
                {
                    Console.Write('|');
                }
                else
                {
                    Console.Write(' ');
                }
            }
        }
        Console.WriteLine();
    }
}

var result = input.Count();

long possiblePeriod = windForecast.Count * rocks.Count;

Dictionary<long, long> prevHeights = new Dictionary<long, long>();
Dictionary<string, long> prevShapes = new Dictionary<string, long>();
prevHeights.Add(0, 0);

(string, long) AirSnapshot()
{
    HashSet<Point> air = new HashSet<Point>();
    HashSet<Point> visited = new HashSet<Point>();
    HashSet<Point> frontier = new HashSet<Point>();
    Point translation = new Point(1, maxHeight);
    for (int x = 1; x < 8; ++x)
    {
        Point potentialStart = new Point(x, maxHeight);
        frontier.Add(potentialStart);
    }
    while (frontier.Count > 0)
    {
        Point current = frontier.First();
        frontier.Remove(current);
        visited.Add(current);
        if (!oldRocks.Contains(current))
        {
            air.Add(current);
            if (current.x > 1)
            {
                Point candidate = current + new Point(-1, 0);
                if (!visited.Contains(candidate))
                {
                    frontier.Add(candidate);
                }
            }
            if (current.x < 7)
            {
                Point candidate = current + new Point(1, 0);
                if (!visited.Contains(candidate))
                {
                    frontier.Add(candidate);
                }
            }
            if (current.y > 1)
            {
                Point candidate = current + new Point(0, -1);
                if (!visited.Contains(candidate))
                {
                    frontier.Add(candidate);
                }
            }
        }
    }
    long rows = maxHeight - air.Min(p => p.y) + 1;
    List<char> snapshot = new List<char>();
    for (int r = 0; r < rows; ++r)
    {
        snapshot.Add('|');
        for (int x = 1; x < 8; ++x)
        {
            if (air.Contains(new Point(x, maxHeight - r)))
            {
                snapshot.Add('.');
            }
            else
            {
                snapshot.Add('#');
            }
        }
        snapshot.Add('|');
        snapshot.Add('\n');
    }
    return (new string(snapshot.ToArray()), rows);
}

void CleanupOldRocks(long relevantHeight) {
    oldRocks = oldRocks.Where(p => p.y + relevantHeight + 2 >= maxHeight).ToHashSet();
}

long limit = 1000000000000;
int progressCounter = 0;
for (long i = 0; i < limit; i++)
{
    bool periodFound = false;
    if (i % possiblePeriod == limit % possiblePeriod && i != 0)
    {
        Console.Write('.');
        progressCounter++;
        if (progressCounter == 25)
        {
            Console.WriteLine();
            progressCounter = 0;
        }
        (string airSnapshot, long relevantHeight) = AirSnapshot();

        CleanupOldRocks(relevantHeight);

        if (prevShapes.ContainsKey(airSnapshot))
        {
            long previousTime = prevShapes[airSnapshot];
            long period = i - prevShapes[airSnapshot];
            long periodDelta = maxHeight - prevHeights[previousTime];
            Console.WriteLine($"We seem to have a repeat!");
            Console.WriteLine($"Steps {i} and {prevShapes[airSnapshot]} share a shape, {i - prevShapes[airSnapshot]} steps apart");
            Console.WriteLine($"In that many steps the height increased by {maxHeight - prevHeights[prevShapes[airSnapshot]]} units");
            if (i % period == limit % period)
            {
                Console.WriteLine("===================================");
                Console.WriteLine($"Answer is expected to be {((limit - i) / period) * periodDelta + maxHeight}");
                Console.WriteLine("===================================");
            }
            prevShapes[airSnapshot] = i;
        }
        else
        {
            prevShapes.Add(airSnapshot, i);
        }
        prevHeights.Add(i, maxHeight);
/*
        for (int p = 1; p < 200; ++p)
        {
            long prev1 = i - p * possiblePeriod;
            long prev2 = i - 2 * p * possiblePeriod;
            long prev3 = i - 3 * p * possiblePeriod;
            if (prevHeights.ContainsKey(prev3))
            {
                long delta1 = maxHeight - prevHeights[prev1];
                long delta2 = prevHeights[prev1] - prevHeights[prev2];
                long delta3 = prevHeights[prev3] - prevHeights[prev2];
                if (delta1 == delta2 && delta1 == delta3)
                {
                    Console.WriteLine($"At step {i} possible period detected at period {p} x {possiblePeriod} = {p * possiblePeriod}");
                }
            }
        }
*/
    }

    List<Point> stone = GetNextStone();
    bool settled = false;
    while (!settled)
    {
        Point wind = GetNextWind();
        List<Point> buffeted = stone.Select(p => p + wind).ToList();
        if (!Collides(buffeted))
        {
            stone = buffeted;
        }
        List<Point> fallen = stone.Select(p => p + down).ToList();
        if (!Collides(fallen))
        {
            stone = fallen;
        }
        else
        {
            foreach (var p in stone)
            {
                oldRocks.Add(p);
                if (p.y > maxHeight)
                {
                    maxHeight = p.y;
                }
            }
            settled = true;
        }
    }
    /*
    ShowRocks();
    Console.WriteLine();
    Console.WriteLine("=====================================");
    Console.WriteLine();
    */
}

Console.WriteLine(maxHeight);

public record Point(long x, long y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}


