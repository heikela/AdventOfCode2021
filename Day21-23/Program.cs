using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

var lines = File.ReadAllLines(fileName);

Dictionary<Point, char> map = new Dictionary<Point, char>();

for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        map[new Point(x, y)] = lines[y][x];
    }
}

Point start = map.First(kv => kv.Value == 'S').Key;
map[start] = '.';

HashSet<Point> current = new HashSet<Point>();
HashSet<Point> next = new HashSet<Point>();

Point up = new Point(0, -1);
Point down = new Point(0, 1);
Point left = new Point(-1, 0);
Point right = new Point(1, 0);

List<Point> dirs = new List<Point>() { up, down, left, right };


current.Add(start);

for (int i = 0; i < 64; ++i)
{
    next = new HashSet<Point>();
    foreach(var p in current)
    {
        foreach (var dir in dirs)
        {
            Point nextP = p + dir;
            if (map.GetOrElse(nextP, '#') != '#')
            {
                next.Add(nextP);
            }
        }
    }
    current = next;
}
Console.WriteLine($"Part 1: {current.Count}");

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}