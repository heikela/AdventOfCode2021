using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

string[] lines = File.ReadAllLines(fileName);

Dictionary<Point, char> contraption = new Dictionary<Point, char>();

for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        contraption[new Point(x, y)] = lines[y][x];
    }
}

Point left = new Point(-1, 0);
Point right = new Point(1, 0);
Point up = new Point(0, -1);
Point down = new Point(0, 1);

IEnumerable<Point> NewDirections(Point currentDirection, char space)
{
    switch (space)
    {
        case '.':
            yield return currentDirection;
            break;
        case '|':
            if (currentDirection == up || currentDirection == down)
            {
                yield return currentDirection;
            }
            else
            {
                yield return up;
                yield return down;
            }
            break;
        case '-':
            if (currentDirection == left || currentDirection == right)
            {
                yield return currentDirection;
            }
            else
            {
                yield return left;
                yield return right;
            }
            break;
        case '/':
            if (currentDirection == up)
            {
                yield return right;
            }
            else if (currentDirection == down)
            {
                yield return left;
            }
            else if (currentDirection == left)
            {
                yield return down;
            }
            else if (currentDirection == right)
            {
                yield return up;
            }
            break;
        case '\\':
            if (currentDirection == up)
            {
                yield return left;
            }
            else if (currentDirection == down)
            {
                yield return right;
            }
            else if (currentDirection == left)
            {
                yield return up;
            }
            else if (currentDirection == right)
            {
                yield return down;
            }
            break;
    }
}

int CountEnergized(Point startPos, Point startDirection)
{
    HashSet<Point> energized = new HashSet<Point>();
    HashSet<PosAndDir> handled = new HashSet<PosAndDir>();

    void FollowBeam(Point pos, Point direction)
    {
        if (handled.Contains(new PosAndDir(pos, direction)))
        {
            return;
        }
        handled.Add(new PosAndDir(pos, direction));
        if (!contraption.ContainsKey(pos))
        {
            return;
        }
        char space = contraption[pos];
        energized.Add(pos);
        foreach (Point newDirection in NewDirections(direction, space))
        {
            Point newPos = pos + newDirection;
            FollowBeam(newPos, newDirection);
        }
    }

    FollowBeam(startPos, startDirection);
    return energized.Count;
}

Console.WriteLine($"Part 1: {CountEnergized(new Point(0, 0), right)}");

IEnumerable<PosAndDir> PossibleStarts()
{
    for (int x = 0; x < lines[0].Length; x++)
    {
        yield return new PosAndDir(new Point(x, 0), down);
        yield return new PosAndDir(new Point(x, lines.Length - 1), up);
    }
    for (int y = 0; y < lines.Length; y++)
    {
        yield return new PosAndDir(new Point(0, y), right);
        yield return new PosAndDir(new Point(lines[0].Length - 1, y), left);
    }
}

Console.WriteLine($"Part 2: {PossibleStarts().Select(p => CountEnergized(p.Pos, p.Dir)).Max()}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}

public record PosAndDir(Point Pos, Point Dir);
