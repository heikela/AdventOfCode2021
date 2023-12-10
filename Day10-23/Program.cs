using Common;
using System.Diagnostics;

//string fileName = "../../../testInput.txt";

string fileName = "../../../input.txt";

//string fileName = "../../../p2test1.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

Point left = new Point(-1, 0);
Point up = new Point(0, -1);
Point right = new Point(1, 0);
Point down = new Point(0, 1);

List<Point> directions = new List<Point>() { left, up, right, down };

Point startPos = new Point(-1, -1);

Dictionary<Point, char> scan = new Dictionary<Point, char>();
int y = 0;
foreach (var line in lines)
{
    int x = 0;
    foreach (char c in line)
    {
        if (c == 'S')
        {
            startPos = new Point(x, y);
        }
        scan.Add(new Point(x, y), c);
        ++x;
    }
    ++y;
}

IEnumerable<Point> neighbours(Point point)
{
    char c = scan.GetOrElse(point, '.');
    return neighbourDirections(c).Select(d => d + point);
}

IEnumerable<Point> neighbourDirections(char c)
{
    switch (c)
    {
        case '-':
            yield return left;
            yield return right;
            break;
        case '|':
            yield return down;
            yield return up;
            break;
        case 'F':
            yield return right;
            yield return down;
            break;
        case '7':
            yield return left;
            yield return down;
            break;
        case 'J':
            yield return up;
            yield return left;
            break;
        case 'L':
            yield return up;
            yield return right;
            break;
        default:
            yield break;
    }
}

char determinePipeAtStart()
{
    List<Point> neighbourDirections = directions.Where(d => neighbours(startPos + d).Contains(startPos)).ToList();
    Debug.Assert(neighbourDirections.Count == 2);
    if (neighbourDirections.Contains(up))
    {
        if (neighbourDirections.Contains(left))
        {
            return 'J';
        }
        else if (neighbourDirections.Contains(down))
        {
            return '|';
        }
        else if (neighbourDirections.Contains(right))
        {
            return 'L';
        }
    }
    else if (neighbourDirections.Contains(left))
    {
        if (neighbourDirections.Contains(down))
        {
            return '7';
        }
        else if (neighbourDirections.Contains(right))
        {
            return '-';
        }
    }
    else
    {
        Debug.Assert(neighbourDirections.Contains(down));
        Debug.Assert(neighbourDirections.Contains(right));
        return 'F';
    }
    throw new ArgumentOutOfRangeException($"Invalid neighbours {neighbourDirections}");
}

scan[startPos] = determinePipeAtStart();

Point furthestPoint = startPos;
int furthestDistance = 0;

Graph<Point> pipe = new GraphByFunction<Point>(neighbours);

void trackFurthest(Point point, Graph<Point>.VisitPath path)
{
    int dist = path.GetLength();
    if (dist > furthestDistance)
    {
        furthestDistance = dist;
        furthestPoint = point;
    }
}

pipe.BfsFrom(startPos, trackFurthest);

HashSet<Point> partOfLoop = new HashSet<Point>();
partOfLoop.Add(startPos);
void AddToLoop(Point point, Graph<Point>.VisitPath path)
{
    partOfLoop.Add(point);
}

pipe.BfsFrom(startPos, AddToLoop);

HashSet<Point> visited = new HashSet<Point>();
HashSet<Point> interior = new HashSet<Point>();

IEnumerable<Point> potentialInteriorNeighbours(Point point)
{
    foreach (Point p in directions.Select(d => d + point))
    {
        if (!partOfLoop.Contains(p))
        {
            yield return p;
        }
    }
}

Graph<Point> potentialInterior = new GraphByFunction<Point>(potentialInteriorNeighbours);

void addToInterior(Point p, GraphByFunction<Point>.VisitPath path)
{
    if (!scan.ContainsKey(p))
    {
        throw new ArgumentOutOfRangeException($"Interior is not interior, trying to add {p}");
    }
    interior.Add(p);
    //Console.WriteLine($"Adding {p}");
}

void processPotentialInterior(Point current, Point inwards)
{
    if (!scan.ContainsKey(current))
    {
        throw new ArgumentOutOfRangeException($"Invalid current point {current}");
    }
    Point candidate = current + inwards;
    //Console.WriteLine($"Searching from {candidate}");
    if (!interior.Contains(candidate) && !partOfLoop.Contains(candidate))
    {
        interior.Add(candidate);
        potentialInterior.BfsFrom(candidate, addToInterior);
    }
}

int size = lines.Length;
for (y = 0; y < size; ++y)
{
    for (int x = 0; x < size; ++x)
    {
        char c = '.';
        Point pos = new Point(x, y);
        if (partOfLoop.Contains(pos))
        {
            c = scan[pos];
        }
        Console.Write(c);
    }
    Console.WriteLine();
}


Point topLeft = partOfLoop.OrderBy(p => p.Y * 150 + p.X).First();
//Console.WriteLine($"Bottom left = {bottomLeft}");
Point current = topLeft + down;
Point inwards = right;

bool done = false;
visited.Add(topLeft);
while (!done)
{
//    Console.WriteLine($"Following the pipe at {current}, finding {scan[current]}");
    visited.Add(current);
    processPotentialInterior(current, inwards);
    switch (scan[current])
    {
        case 'J':
        case 'F':
//            Console.WriteLine($"Turning based on {scan[current]}");
//            Console.WriteLine($"From inwards = {inwards}");
            if (inwards == up)
            {
                inwards = left;
            }
            else if (inwards == left)
            {
                inwards = up;
            }
            else if (inwards == right)
            {
                inwards = down;
            }
            else if (inwards == down)
            {
                inwards = right;
            }
//            Console.WriteLine($"To inwards = {inwards}");
            break;
        case '7':
        case 'L':
//            Console.WriteLine($"Turning based on {scan[current]}");
//            Console.WriteLine($"From inwards = {inwards}");
            if (inwards == up)
            {
                inwards = right;
            }
            else if (inwards == right)
            {
                inwards = up;
            }
            else if (inwards == left)
            {
                inwards = down;
            }
            else if (inwards == down)
            {
                inwards = left;
            }
//            Console.WriteLine($"To inwards = {inwards}");
            break;
        default:
            break;
    }
    processPotentialInterior(current, inwards);
    var next = neighbours(current).Where(p => !visited.Contains(p));
    if (!next.Any())
    {
        done = true;
    }
    else
    {
        current = next.First();
    }
}

Console.WriteLine($"The furthest point in the loop is at a distance of {furthestDistance}");
Console.WriteLine($"Interior of the loop is size {interior.Count}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
}

