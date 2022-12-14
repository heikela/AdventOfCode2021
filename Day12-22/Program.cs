using Common;

//string inputFile = "../../../testInput.txt";
string inputFile = "../../../input.txt";

int y = 0;

Point start = new Point(-1, -1);
Point end = new Point(-1, -1);

Dictionary<Point, int> height = new Dictionary<Point, int>();

List<Point> Directions = new List<Point>() {
    new Point(-1, 0),
    new Point(1, 0),
    new Point(0, -1),
    new Point(0, 1)
};

foreach(string line in File.ReadAllLines(inputFile))
{
    int x = 0;
    foreach (char c in line)
    {
        if (c == 'S')
        {
            height.Add(new Point(x, y), 0);
            start = new Point(x, y);
        }
        else if (c == 'E')
        {
            height.Add(new Point(x, y), 'z' - 'a');
            end = new Point(x, y);
        }
        else
        {
            height.Add(new Point(x, y), c - 'a');
        }
        ++x;
    }
    ++y;
}


GraphByFunction<Point> paths = new GraphByFunction<Point>(p => Directions.Select(d => d + p).Where(p2 => height.ContainsKey(p2) && height[p2] <= height[p] + 1));
int pathLen = paths.ShortestPathTo(start, end).GetLength();
Console.WriteLine(pathLen);

GraphByFunction<Point> betterPaths = new GraphByFunction<Point>(p => Directions.Select(d => d + p).Where(p2 => height.ContainsKey(p2) && height[p2] >= height[p] - 1));
pathLen = betterPaths.ShortestPathTo(end, p => height[p] == 0).GetLength();
Console.WriteLine(pathLen);

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}

