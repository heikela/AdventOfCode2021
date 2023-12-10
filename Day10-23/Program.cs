using Common;

//string fileName = "../../../testInput.txt";
//char S = 'F';
string fileName = "../../../input.txt";
char S = 'L';

string[] lines = File.ReadAllLines(fileName).ToArray();

Point left = new Point(-1, 0);
Point up = new Point(0, -1);
Point right = new Point(1, 0);
Point down = new Point(0, 1);

Point startPos = new Point(-1, -1);

Dictionary<Point, char> scan = new Dictionary<Point, char>();
int y = 0;
foreach (var line in lines)
{
    int x = 0;
    foreach (char c in line)
    {
        scan.Add(new Point(x, y), c);
        if (c == 'S')
        {
            startPos = new Point(x, y);
        }
        ++x;
    }
    ++y;
}

IEnumerable<Point> neighbours(Point point)
{
    char c = scan[point];
    if (c == 'S')
    {
        c = S;
    }
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

Console.WriteLine($"The furthest point in the loop is at a distance of {furthestDistance}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
}

