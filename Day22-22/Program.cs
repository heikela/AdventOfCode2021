using Common;

//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

List<Point> directions = new List<Point>()
{
    new Point(1, 0),
    new Point(0, 1),
    new Point(-1, 0),
    new Point(0, -1)
};

Point dir = directions[0];

Dictionary<Point, char> map = new Dictionary<Point, char>();

int y = 1;
foreach (var line in input.Paragraphs().First())
{
    int x = 1;
    foreach (char c in line)
    {
        if (c != ' ')
        {
            map.Add(new Point(x, y), c);
        }
        ++x;
    }
    ++y;
}

Dictionary<Point, char> mapWithPath = map.ToDictionary();


int minX = map.Keys.Select(p => p.x).Min();
int maxX = map.Keys.Select(p => p.x).Max();
int minY = map.Keys.Select(p => p.y).Min();
int maxY = map.Keys.Select(p => p.y).Max();

Point pos = Wrap(new Point(1, 1), new Point(1, 0));

Console.WriteLine($"StartPos = {pos}");

string instructions = input.Paragraphs().Skip(1).First().First();

Console.WriteLine(instructions);

int instrPos = 0;
mapWithPath[pos] = VisualizeDir(dir);
int iCount = 0;
while (instrPos < instructions.Length)
{
    iCount++;
    char current = instructions[instrPos];
    if (current == 'L')
    {
        dir = Left(dir);
        ++instrPos;
        mapWithPath[pos] = VisualizeDir(dir);
//        Console.WriteLine("Left");
    }
    else if (current == 'R')
    {
        dir = Right(dir);
        ++instrPos;
        mapWithPath[pos] = VisualizeDir(dir);
//        Console.WriteLine("Right");
    }
    else if (current >= '0' && current <= '9')
    {
        string numChars = new string(instructions.Skip(instrPos).TakeWhile(c => c >= '0' && c <= '9').ToArray());
        int fwd = int.Parse(numChars);
        for (int i = 0; i < fwd; i++)
        {
            (pos, dir) = Forward2(pos, dir);
            mapWithPath[pos] = VisualizeDir(dir);
//            Console.Write('F');
        }
        instrPos += numChars.Length;
//        Console.WriteLine($" - {fwd}");
    }
    if (iCount % 20 == 0 && iCount < 110)
    {
        Console.WriteLine();
        Console.WriteLine("===============================================================");
        Console.WriteLine("===============================================================");
        Console.WriteLine();
        VisualizeMap(mapWithPath);
    }
}

VisualizeMap(mapWithPath);

/*
 *     aaabbb
 *    e111222c
 *    e111222c
 *    e111222c
 *    f333ddd
 *    f333d
 *  fff333d
 * e444555c
 * e444555c
 * e444555c
 * a666ggg
 * a666g
 * a666g
 *  bbb
 *  
 */


(Point, Point) Wrap2(Point p, Point d)
{
    if (p.y == 1 && p.x > 50 && p.x <= 100 && d == new Point(0, -1))
    // up on edge a
    {
        return (new Point(1, 150 + p.x - 50), Right(d));
    }
    if (p.x == 1 && p.y > 150 && p.y <= 200 && d == new Point(-1, 0))
    // edge a from 6
    {
        return (new Point(p.y - 100, 1), Left(d));
    }
    if (p.y == 1 && p.x > 100 && p.x <= 150 && d == new Point(0, -1))
    // up on edge b
    {
        return (new Point(p.x - 100, 200), d);
    }
    if (p.y == 200 && p.x > 0 && p.x <= 50 && d == new Point(0, 1))
    // edge b from 6
    {
        return (new Point(p.x + 100, 1), d);
    }
    if (p.x == 150 && p.y > 0 && p.y <= 50 && d == new Point(1, 0))
    {
        // edge c from 2
        return (new Point(100, 151 - p.y), Right(Right(d)));
    }
    if (p.x == 100 && p.y > 100 && p.y <= 150 && d == new Point(1, 0))
    {
        // edge c from 5
        return (new Point(150, 51 - (p.y - 100)), Right(Right(d)));
    }
    if (p.y == 50 && p.x > 100 && p.x <= 150 && d == new Point(0, 1))
    // edge d from 2 ?? Copied
    {
        return (new Point(100, p.x - 50), Right(d));
    }
    if (p.x == 100 && p.y > 50 && p.y <= 100 && d == new Point(1, 0))
    // edge d from 3 ?? Copied
    {
        return (new Point(p.y + 50, 50), Left(d));
    }
    if (p.x == 51 && p.y > 0 && p.y <= 50 && d == new Point(-1, 0))
    {
        // edge e from 1
        return (new Point(1, 151 - p.y), Right(Right(dir)));
    }
    if (p.x == 1 && p.y > 100 && p.y <= 150 && d == new Point(-1, 0))
    {
        // edge e from 4
        return (new Point(51, 151 - p.y), Right(Right(dir)));
    }
    if (p.y == 101 && p.x > 0 && p.x <= 50 && d == new Point(0, -1))
    {
        // edge f from 4
        return (new Point(51, p.x + 50), Right(d));
    }
    if (p.x == 51 && p.y > 50 && p.y <= 100 && d == new Point(-1, 0))
    {
        // edge f from 3
        return (new Point(p.y - 50, 101), Left(d));
    }
    if (p.y == 150 && p.x > 50 && p.x <= 100 && d == new Point(0, 1))
    {
        // edge g from 5
        return (new Point(50, p.x + 100), Right(d));
    }
    if (p.x == 50 && p.y > 150 && p.y <= 200 && d == new Point(1, 0))
    {
        // edge g from 6
        return (new Point(p.y - 100, 150), Left(d));
    }

    throw new Exception($"Unexpected Wrap request at pos = {p}, dir = {d}");
}


void VisualizeMap(Dictionary<Point, char> map)
{
    for (int y = minY; y < maxY; y++)
    {
        for (int x = minX; x < maxX; x++)
        {
            Console.Write(map.GetOrElse(new Point(x, y), ' '));
        }
        Console.WriteLine();
    }
}

Point Wrap(Point current, Point dir)
{
    int x, y;
    x = current.x;
    y = current.y;
    if (dir.x == 0)
    {
        if (dir.y < 0)
        {
            y = maxY;
            while (!map.ContainsKey(new Point(x, y)))
            {
                --y;
            }
        }
        else
        {
            y = minY;
            while (!map.ContainsKey(new Point(x, y)))
            {
                ++y;
            }
        }
    }
    else
    {
        if (dir.x < 0)
        {
            x = maxX;
            while (!map.ContainsKey(new Point(x, y)))
            {
                --x;
            }
        }
        else
        {
            x = minX;
            while (!map.ContainsKey(new Point(x, y)))
            {
                ++x;
            }
        }
    }
    return new Point(x, y);
}

Point Forward(Point current, Point dir)
{
    Point next = current + dir;
    if (!map.ContainsKey(next))
    {
        next = Wrap(current, dir);
    }
    if (map[next] == '#')
    {
        return current;
    }
    else
    {
        return next;
    }
}

(Point, Point) Forward2(Point current, Point dir)
{
    Point next = current + dir;
    Point newDir = dir;
    if (!map.ContainsKey(next))
    {
        (next, newDir) = Wrap2(current, dir);
        Point testDir = Right(Right(newDir));
        (Point testPos, Point verifyDir) = Wrap2(next, testDir);
        verifyDir = Right(Right(verifyDir));
        if (testPos != current || verifyDir != dir)
        {
            throw new Exception("Wrapping around doesn't seem to be reversible");
        }
    }
    if (map[next] == '#')
    {
        return (current, dir);
    }
    else
    {
        return (next, newDir);
    }
}

Point Left(Point dir)
{
    int dirIndex = directions.IndexOf(dir);
    dirIndex = (dirIndex - 1 + directions.Count) % directions.Count;
    return directions[dirIndex];
}

Point Right(Point dir)
{
    int dirIndex = directions.IndexOf(dir);
    dirIndex = (dirIndex + 1) % directions.Count;
    return directions[dirIndex];
}

char VisualizeDir(Point dir)
{
    if (dir == new Point(1, 0))
    {
        return '>';
    }
    if (dir == new Point(-1, 0))
    {
        return '<';
    }
    if (dir == new Point(0, 1))
    {
        return 'v';
    }
    if (dir == new Point(0, -1))
    {
        return '^';
    }
    throw new Exception($"Bad dir {dir}");
}




int row = pos.y;
int column = pos.x;
int heading = directions.IndexOf(dir);


int password = 1000 * row + 4 * column + heading;
Console.WriteLine(password);

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
}

