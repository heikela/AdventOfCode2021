using Common;

string[] lines = new AoCUtil().GetInput(2023, 11);
//string[] lines = new AoCUtil().GetTestBlock(2023, 11, 0);

List<Point> starsAsScanned = new List<Point>();
int y = 0;
foreach (var line in lines)
{
    int x = 0;
    foreach (char c in line)
    {
        if (c == '#')
        starsAsScanned.Add(new Point(x, y));
        ++x;
    }
    ++y;
}

int maxX = starsAsScanned.Max(p => p.X);
int maxY = starsAsScanned.Max(p => p.Y);

List<int> emptyX = Enumerable.Range(0, maxX + 1).Where(x => starsAsScanned.Count(p => p.X == x) == 0).ToList();
List<int> emptyY = Enumerable.Range(0, maxY + 1).Where(y => starsAsScanned.Count(p => p.Y == y) == 0).ToList();

long starDistance(Point a, Point b, int expansionFactor)
{
    long unexpanded = Point.ManhattanDistance(a, b);
    int smallerX = Math.Min(a.X, b.X);
    int smallerY = Math.Min(a.Y, b.Y);
    int largerX = Math.Max(a.X, b.X);
    int largerY = Math.Max(a.Y, b.Y);
    long expanded = unexpanded + (expansionFactor - 1) * (emptyX.Count(x => x > smallerX && x < largerX) + emptyY.Count(y => y > smallerY && y <largerY));
    return expanded;
}

long distanceSum1 = 0;
long distanceSum2 = 0;
for (int i = 0; i < starsAsScanned.Count; i++)
{
    for (int j = i + 1; j < starsAsScanned.Count; j++)
    {
        distanceSum1 += starDistance(starsAsScanned[i], starsAsScanned[j], 2);
        distanceSum2 += starDistance(starsAsScanned[i], starsAsScanned[j], 1000000);
    }
}

Console.WriteLine(distanceSum1);
Console.WriteLine(distanceSum2);

public record Point(int X, int Y)
{
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static int ManhattanDistance(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
