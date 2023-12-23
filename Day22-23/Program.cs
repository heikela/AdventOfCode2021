using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

Point down = new Point(0, 0, -1);
Point up = new Point(0, 0, 1);

var lines = File.ReadAllLines(fileName);

var bricks = lines.Select(line =>
{
    var parts = line.Split('~');
    var a = parts[0].Split(',').Select(int.Parse).ToArray();
    var b = parts[1].Split(',').Select(int.Parse).ToArray();
    return new Brick(new Point(a[0], a[1], a[2]), new Point(b[0], b[1], b[2]));
}).OrderBy(b => b.MinZ()).ToArray();

Dictionary<Point, int> brickByPos = new Dictionary<Point, int>();

for (int i = 0; i < bricks.Length; ++i)
{
    Brick b = bricks[i];
    foreach (Point p in b.GetPoints())
    {
        brickByPos.Add(p, i);
    }
}

bool canFall(Brick brick)
{
    if (brick.MinZ() == 1)
    {
        return false;
    }
    return brick.GetSupport().All(p => !brickByPos.ContainsKey(p));
}

for (int i = 0; i < bricks.Length; ++i)
{
    Brick b = bricks[i];
    while (canFall(b))
    {
        Brick newB = b.MoveDown();
        bricks[i] = newB;
        foreach (Point p in b.GetTop())
        {
            brickByPos.Remove(p);
        }
        foreach (Point p in b.GetSupport())
        {
            brickByPos.Add(p, i);
        }
        b = newB;
    }
}

HashSet<int> supporters(Brick brick)
{
    return brick.GetSupport().Where(p => brickByPos.ContainsKey(p)).Select(p => brickByPos[p]).ToHashSet();
}

bool canBeDisintegrated(Brick brick)
{
    return brick.GetAbove().All(p => !brickByPos.ContainsKey(p) || supporters(bricks[brickByPos[p]]).Count > 1);
}

int canBeDisintegratedIndividually = bricks.Count(canBeDisintegrated);

Console.WriteLine($"Part 1: {canBeDisintegratedIndividually}");

int countFallingIfDisintegrated(int idx)
{
    HashSet<int> disturbed = new HashSet<int>();
    disturbed.Add(idx);
    for (int i = idx; i < bricks.Length; ++i)
    {
        Brick b = bricks[i];
        HashSet<int> bricksAbove = b.GetAbove().Where(p => brickByPos.ContainsKey(p)).Select(p => brickByPos[p]).ToHashSet();
        foreach (int brickAbove in bricksAbove)
        {
            if (supporters(bricks[brickAbove]).All(i => disturbed.Contains(i)))
            {
                disturbed.Add(brickAbove);
            }
        }
    }
    return disturbed.Count - 1;
}

int fallingSum = 0;
for (int i = 0; i < bricks.Length; ++i)
{
    fallingSum += countFallingIfDisintegrated(i);
}

Console.WriteLine(fallingSum);

public record Brick(Point A, Point B)
{
    public IEnumerable<Point> GetPoints()
    {
        int MinX = Math.Min(A.X, B.X);
        int MaxX = Math.Max(A.X, B.X);
        int MinY = Math.Min(A.Y, B.Y);
        int MaxY = Math.Max(A.Y, B.Y);
        int MinZ = Math.Min(A.Z, B.Z);
        int MaxZ = Math.Max(A.Z, B.Z);

        for (int x = MinX; x <= MaxX; x++)
        {
            for (int y = MinY; y <= MaxY; y++)
            {
                for (int z = MinZ; z <= MaxZ; z++)
                {
                    yield return new Point(x, y, z);
                }
            }
        }
    }

    public IEnumerable<Point> GetSupport()
    {
        HashSet<Point> current = GetPoints().ToHashSet();
        return current.Select(p => p + new Point(0, 0, -1)).Where(p => !current.Contains(p));
    }

    public IEnumerable<Point> GetAbove()
    {
        HashSet<Point> current = GetPoints().ToHashSet();
        return current.Select(p => p + new Point(0, 0, 1)).Where(p => !current.Contains(p));
    }

    public IEnumerable<Point> GetTop()
    {
        HashSet<Point> current = GetPoints().ToHashSet();
        return current.Where(p => !current.Contains(p + new Point(0, 0, 1)));
    }

    public Brick MoveDown()
    {
        return new Brick(A + new Point(0, 0, -1), B + new Point(0, 0, -1));
    }

    public int MinZ()
    {
        return Math.Min(A.Z, B.Z);
    }

}

public record Point(int X, int Y, int Z)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
}

