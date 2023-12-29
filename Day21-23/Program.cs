using Common;
using System.Diagnostics;

string[] lines = new AoCUtil().GetInput(2023, 21);
//string[] lines = new AoCUtil().GetTestBlock(2023, 21, 0);

Dictionary<Point, char> map = new Dictionary<Point, char>();

int W = lines[0].Length;
int H = lines.Length;
Debug.Assert(W == H);

for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        map[new Point(x, y)] = lines[y][x];
    }
}

Point start = map.First(kv => kv.Value == 'S').Key;
map[start] = '.';
Debug.Assert(W == start.X * 2 + 1);
Debug.Assert(H == start.Y * 2 + 1);

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

HashSet<Point> visitedInFirstMap = new HashSet<Point>();
Dictionary<Point, Dictionary<Point, int>> distDiffs = new Dictionary<Point, Dictionary<Point, int>>();
int maxDelay = 0;

void recordVisit(Point p, int dist)
{
    Point indexOfMap = mapIndex(p);
    Point indexInMap = p - indexOfMap * W;
    if (indexOfMap == new Point(0, 0))
    {
        visitedInFirstMap.Add(p);
        Debug.Assert(indexInMap == p);
    }
    int distDiff = dist - p.ManhattanDistance(start);
    if (distDiff > maxDelay)
    {
        maxDelay = distDiff;
    }
    if (!distDiffs.ContainsKey(indexInMap))
    {
        distDiffs.Add(indexInMap, new Dictionary<Point, int>() { { indexOfMap, distDiff } });
    }
    else
    {
        var innerDict = distDiffs[indexInMap];
        if (innerDict.ContainsKey(indexOfMap))
        {
            Debug.Assert(innerDict[indexOfMap] <= distDiff);
        }
        else
        {
            innerDict.Add(indexOfMap, distDiff);
        }
    }
}

void explore()
{
    visitedInFirstMap.Add(start);
    HashSet<Point> current = new HashSet<Point>() { start };
    HashSet<Point> saturated = new HashSet<Point>();
    long saturatedExcluded = 0;

    recordVisit(start, 0);
    for (int i = 0; i < 1800; i += 2)
    {
        (current, saturated, saturatedExcluded) = twoStepsFrom(current, saturated, saturatedExcluded, i);
    }

    for (int mx = -6; mx < 7; ++mx)
    {
        for (int my = -6; my < 7; ++my)
        {
            Point mapPos = new Point(mx, my);
            Point clampedMapPos = mapPos.clamp(4);
            for (int x = 0; x < W; ++x)
            {
                for (int y = 0; y < H; ++y)
                {
                    Point posInMap = new Point(x, y);

                    if (distDiffs.ContainsKey(posInMap) && distDiffs[posInMap].ContainsKey(mapPos))
                    {

                        int correctDd = distDiffs[posInMap][mapPos];
                        int clampedDd = distDiffs[posInMap][clampedMapPos];
                        if (correctDd != clampedDd)
                        {
                            Console.WriteLine($"Clamping map positions doesn't work at pos {posInMap}, map index {mapPos}. Distance diff is {correctDd} but at map index {clampedMapPos} it is {clampedDd}");
                        }
                    }
                }
            }
        }
    }


    var controversial = distDiffs.Where(kv => kv.Value.Count() > 1).ToDictionary();

    Console.WriteLine($"Exploration done, {controversial.Count} controversial entries");
}

explore();

void calculate(int moves)
{
    int mapSizesDefinitelyIn = Math.Max(0, (moves - maxDelay) / H);
    /*      x
     *     xxx
     *    xxxxx
     *     xxx
     *      x
     * 
        0 => 0;
        1 => 1;
        2 => 5;
        3 => 13;

        (x-1)*(x-1) + x * x
        = 2 x*x - 2x + 1

      1 / 2 * x * (

    0 => 0, 0
    1 => 1, 0
    2 => 1, 0 + 4
    3 => 1 + 8 = 9, 0 + 4 = 4
    4 => 1 + 8 = 9, 4 + 12 = 16
    5 => 9 + 16 = 25, 4 + 12 = 16
    6 => 9 + 16 = 25, 16 + 20 = 36
    7 => 25 + 24 = 49,


    */

    long parityZeroMapsDefinitelyIn = 0L;
    long parityOneMapsDefinitelyIn = 0L;

    long c = ((long)mapSizesDefinitelyIn + 1L) / 2;
    long d = Math.Max(0, 2 * c - 1);
    parityZeroMapsDefinitelyIn = d * d;

    c = (long)mapSizesDefinitelyIn / 2L;
    parityOneMapsDefinitelyIn = (2 * c) * (2 * c);

    Console.WriteLine(parityZeroMapsDefinitelyIn);
    Console.WriteLine(parityOneMapsDefinitelyIn);

    /*
        long mapsDefinitelyIn = 2L * mapSizesDefinitelyIn * mapSizesDefinitelyIn - 2L * mapSizesDefinitelyIn + 1L;
        if (mapSizesDefinitelyIn == 0)
        {
            mapsDefinitelyIn = 0;
        }
    */

    List<Point> directions = new List<Point>() { up, down, left, right, up + right, up + left, down + right, down + left };
    List<Point> diagonals = new List<Point>() { up + right, up + left, down + right, down + left };

    long calculateShell(int moves, int d)
    {
        Dictionary<Point, long> countsByDirection = new Dictionary<Point, long>();
        foreach (Point dir in directions)
        {
            countsByDirection.Add(dir, 0);
        }
        Point topIndex = new Point(0, -d);
        Point leftIndex = new Point(-d, 0);
        Point bottomIndex = new Point(0, d);
        Point rightIndex = new Point(d, 0);

        Point dirToRepresentativeMapIndex(Point dir)
        {
            if (dir == up)
            {
                return topIndex;
            }
            if (dir == down)
            {
                return bottomIndex;
            }
            if (dir == left)
            {
                return leftIndex;
            }
            if (dir == right)
            {
                return rightIndex;
            }
            if (dir == left + up)
            {
                return topIndex + left + down;
            }
            if (dir == left + down)
            {
                return bottomIndex + left + up;
            }
            if (dir == right + up)
            {
                return topIndex + right + down;
            }
            if (dir == right + down)
            {
                return bottomIndex + right + up;
            }
            throw new Exception($"Invalid direction {dir}");
        }

        for (int y = 0; y < H; ++y)
        {
            for (int x = 0; x < W; ++x)
            {
                Point posInMap = new Point(x, y);
                if (distDiffs.ContainsKey(posInMap))
                {
                    foreach (Point dir in directions)
                    {
                        Point representativeMap = dirToRepresentativeMapIndex(dir);
                        int dist = start.ManhattanDistance(relativeToAbsolute(posInMap, representativeMap));
                        Point clampedDirection = representativeMap.clamp(2);
                        dist += distDiffs[posInMap][clampedDirection];
                        if (dist <= moves && (dist % 2 == moves % 2))
                        {
                            countsByDirection[dir]++;
                        }
                    }
                }
            }
        }
        long diagonalMultiplier = Math.Max(bottomIndex.Y - 1, 0);

        long total = 0;
        total += countsByDirection[up];
        if (bottomIndex != topIndex)
        {
            total += countsByDirection[down];
        }
        if (leftIndex != topIndex)
        {
            total += countsByDirection[left];
        }
        if (rightIndex != leftIndex)
        {
            total += countsByDirection[right];
        }
        total += countsByDirection[right + up] * diagonalMultiplier;
        total += countsByDirection[right + down] * diagonalMultiplier;
        total += countsByDirection[left + up] * diagonalMultiplier;
        total += countsByDirection[left + down] * diagonalMultiplier;
        return total;
    }

    //    Console.WriteLine($"TopIndices = {mapIndex(start + new Point(0, -moves))} to {mapIndex(start + new Point(-start.X, -moves + maxDelay + start.X))}");

    Console.WriteLine($"Calculating for {moves} moves");
    long innerCountP0 = parityZeroMapsDefinitelyIn * visitedInFirstMap.Count(p => (Math.Abs(p.X - start.X) + Math.Abs(p.Y - start.Y) + moves) % 2 == 0);
    long innerCountP1 = parityOneMapsDefinitelyIn * visitedInFirstMap.Count(p => (Math.Abs(p.X - start.X) + Math.Abs(p.Y - start.Y) + moves) % 2 == 1);
    Console.WriteLine($"InnercountP0 = {innerCountP0}");
    Console.WriteLine($"InnercountP1 = {innerCountP1}");

    long total = innerCountP0 + innerCountP1;

    bool moreToAdd = true;
    int dd = 0;
    while (moreToAdd)
    {
        long shellContribution = calculateShell(moves, mapSizesDefinitelyIn + dd);
        ++dd;
        total += shellContribution;
        if (shellContribution == 0)
        {
            moreToAdd = false;
        }
    }

    //    Console.WriteLine($"Part 2 estimate 1 {total}");
    //    Console.WriteLine($"Part 2 estimate 2 {total}");
    //    Console.WriteLine($"Part 2 estimate 3 {total}");
    // 630130522302936 is not right
    // 630129824772393
    Console.WriteLine($"In {moves} steps, {total} locations appear reachable");
}

calculate(6);
calculate(10);
calculate(50);
calculate(100);
calculate(500);
calculate(1000);
calculate(5000);
calculate(26501365);

(HashSet<Point>, HashSet<Point>, long) twoStepsFrom(HashSet<Point> liveStarts, HashSet<Point> saturated, long saturatedExcluded, int step)
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
                    recordVisit(nextP, step + i + 1);
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

Point relativeToAbsolute(Point posInMap, Point mapIndex)
{
    return posInMap + mapIndex * H;
}

/*
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
}*/

/*HashSet<Point> saturated = new HashSet<Point>();
long saturatedExcluded = 0;

for (int i = 0; i < 1000; i += 2)
{
    (current, saturated, saturatedExcluded) = twoStepsFrom(current, saturated, saturatedExcluded);
}


Console.WriteLine($"Part 2: {current.LongCount() + saturated.LongCount() + saturatedExcluded}");
*/

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);

    public int ManhattanDistance(Point other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public static Point operator *(Point a, int b) => new Point(a.X * b, a.Y * b);

    public static Point operator-(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

    public Point clamp(int d)
    {
        return new Point(Math.Min(Math.Max(-d, X), d), Math.Min(Math.Max(-d, Y), d));
    }
}
