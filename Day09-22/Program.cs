var input = File.ReadAllLines("../../../input.txt");

int SimulateRope(IEnumerable<string> moves, int len = 2)
{
    HashSet<Point> visitedByTail = new HashSet<Point>();

    List<Point> rope = new List<Point>();
    for (int i = 0; i < len; i++)
    {
        rope.Add(new Point(0, 0));
    }

    Dictionary<char, Point> directions = new Dictionary<char, Point>();
    directions.Add('L', new Point(-1, 0));
    directions.Add('R', new Point(1, 0));
    directions.Add('U', new Point(0, -1));
    directions.Add('D', new Point(0, 1));

    visitedByTail.Add(rope[len - 1]);

    foreach (var line in moves)
    {
        string[] parts = line.Split(' ');
        for (int i = 0; i < int.Parse(parts[1]); i++)
        {
            rope[0] = rope[0] + directions[parts[0][0]];
            for (int pos = 1; pos < len; pos++)
            {
                Point dist = rope[pos - 1] - rope[pos];
                Point step = Normalize(dist);
                if (step != dist)
                {
                    rope[pos] = rope[pos] + step;
                }
            }
            visitedByTail.Add(rope[len - 1]);
        }
    }
    return visitedByTail.Count;
}

Console.WriteLine(SimulateRope(input, 2));
Console.WriteLine(SimulateRope(input, 10));

Point Normalize(Point p)
{
    return new Point(Math.Max(Math.Min(p.x, 1), -1), Math.Max(Math.Min(p.y, 1), -1));
}

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}
