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

bool withinLimits(Point pos) => pos.X >= 0 && pos.X < W && pos.Y >= 0 && pos.Y < H;

Dictionary<Point, char> roll(Dictionary<Point, char> original, Point direction)
{
    Dictionary<Point, char> result = original.Select(kv => new KeyValuePair<Point, char>(kv.Key, kv.Value == 'O' ? '.' : kv.Value)).ToDictionary();
    foreach (Point rollingRock in original.Where(kv => kv.Value == 'O').Select(kv => kv.Key).OrderByDescending(p => p.InnerProduct(direction)))
    {
        Point newPos = rollingRock;
        while (withinLimits(newPos + direction) && result[newPos + direction] == '.')
        {
            newPos += direction;
        }
        result[newPos] = 'O';
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

Point north = new Point(0, -1);
Point west = new Point(-1, 0);
Point south = new Point(0, 1);
Point east = new Point(1, 0);
List<Point> cycleDirections = new List<Point>() { north, west, south, east };

Dictionary<Point, char> cycle(Dictionary<Point, char> map)
{
    return cycleDirections.Aggregate(map, (acc, direction) => roll(acc, direction));
}

IEnumerable<char> mapChars(Dictionary<Point, char> map)
{
    for (int y = 0; y < H; ++y)
    {
        for (int x = 0; x < W; ++x)
        {
            yield return map[new Point(x, y)];
        }
    }
}

string mapToString(Dictionary<Point, char> map)
{
    return new string(mapChars(map).ToArray());
}

Dictionary<Point, char> newMap = roll(map, north);
//printMap(newMap);
Console.WriteLine(load(roll(map, north)));

Dictionary<string, int> seen = new Dictionary<string, int>();

seen[mapToString(map)] = 0;
bool cycleDetected = false;
int cycleLength = -1;
for (int i = 0; i < 10000; ++i)
{
    map = cycle(map);
    int cycleNumber = i + 1;
    string mapString = mapToString(map);
    if (!cycleDetected && seen.ContainsKey(mapString))
    {
        cycleLength = cycleNumber - seen[mapString];
        cycleDetected = true;
        Console.WriteLine($"Cycle detected at {cycleNumber} with length {cycleLength}");
    }
    if (cycleDetected && cycleNumber % cycleLength == 1000000000 % cycleLength)
    {
        Console.WriteLine($"After {cycleNumber} cycles, load is {load(map)}");
        Console.WriteLine($"This should be the same as after 1000000000 cycles");
        break;
    }
    seen[mapString] = cycleNumber;
}



public record Point(int X, int Y)
{
    public static Point operator+(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public int InnerProduct(Point other) => X * other.X + Y * other.Y;
}
