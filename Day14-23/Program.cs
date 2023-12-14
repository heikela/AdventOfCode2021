using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

Dictionary<Point, char> map = new Dictionary<Point, char>();

int y = 0;
foreach (var line in lines)
{
    for (int x = 0; x < line.Length; x++)
    {
        Point pos = new Point(x, y);
        map[pos] = line[x];
    }
    ++y;
}
int H = y;
int W = lines.First().Length;

bool isEmpty(Point pos) => map[pos] == '.';

Dictionary<Point, char> rollNorth(Dictionary<Point, char> original)
{
    Dictionary<Point, char> result = new Dictionary<Point, char>();
    for (int y = 0; y < H; ++y)
    {
        for (int x = 0; x < W; ++x)
        {
            Point pos = new Point(x, y);
            if (original[pos] == 'O')
            {
                result.Add(pos, '.');
                int newY = y;
                while (newY > 0 && result[new Point(x, newY - 1)] == '.')
                {
                    --newY;
                }
                result[new Point(x, newY)] = 'O';
            }
            else
            {
                result.Add(pos, original[pos]);
            }
        }
    }
    return result;
}

void printMap(Dictionary<Point, char> map)
{
    for (int y = 0; y < H; ++y)
    {
        for (int x = 0; x < W; ++x)
        {
            Console.Write(map[new Point(x, y)]);
        }
        Console.WriteLine();
    }
}

int load(Dictionary<Point, char> map)
{
    return map.Where(kv => kv.Value == 'O').Sum(kv => H - kv.Key.Y);
}

Dictionary<Point, char> newMap = rollNorth(map);
//printMap(newMap);
Console.WriteLine(load(rollNorth(map)));

public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}
