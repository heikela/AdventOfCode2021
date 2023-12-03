using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

Dictionary<Point, char> points = new Dictionary<Point, char>();
Dictionary<List<Point>, int> numbers = new Dictionary<List<Point>, int>();



int y = 0;
foreach (string line in lines)
{
    int x = 0;
    int currentNumber = 0;
    List<Point> currentNumberPositions = new List<Point>();
    foreach (char c in line)
    {
        points.Add(new Point(x, y), c);
        if (isDigit(c))
        {
            currentNumber *= 10;
            currentNumber += getDigit(c);
            currentNumberPositions.Add(new Point(x, y));
        }
        else
        {
            if (currentNumberPositions.Count > 0)
            {
                numbers.Add(currentNumberPositions, currentNumber);
                currentNumber = 0;
                currentNumberPositions = new List<Point>();
            }
        }
        ++x;
    }
    if (currentNumberPositions.Count > 0)
    {
        numbers.Add(currentNumberPositions, currentNumber);
        currentNumber = 0;
        currentNumberPositions = new List<Point>();
    }
    ++y;
}

Point zero = new Point(0, 0);
List<Point> adjacent = Enumerable.Range(-1, 3).SelectMany(dy => Enumerable.Range(-1, 3).Select(dx => new Point(dx, dy))).Where(p => p != zero).ToList();

IEnumerable<Point> neighbours(Point point)
{
    return adjacent.Select(d => d + point);
}


static bool isDigit(char c)
{
    return (c >= '0' && c <= '9');
}

static int getDigit(char c)
{
    if (isDigit(c))
    {
        return c - '0';
    }
    else
    {
        return 0;
    }
}

static bool isSymbol(char c)
{
    return c != '.' && !isDigit(c);
}

Console.WriteLine(numbers.Where(number => number.Key.SelectMany(p => neighbours(p)).Any(n => isSymbol(points.GetOrElse(n, '.')))).Select(p => p.Value).Sum());

IEnumerable<KeyValuePair<List<Point>, int>> numberNeighbours(Point point)
{
    return numbers.Where(n => n.Key.SelectMany(neighbours).Any(numberNeighbour => numberNeighbour == point));
}

var gears = points.Where(p => p.Value == '*').Where(p => numberNeighbours(p.Key).Count() == 2);

Console.WriteLine(gears.Select(g => numberNeighbours(g.Key).Select(number => number.Value).Aggregate(1, (a, b) => a * b)).Sum());

public record Point(int x, int y)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
}
