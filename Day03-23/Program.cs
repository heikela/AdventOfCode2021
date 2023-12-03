using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

Dictionary<Point, char> points = new Dictionary<Point, char>();
List<Number> numbers = new List<Number>();

int y = 0;
foreach (string line in lines)
{
    int x = 0;
    Number currentNumber = new Number();
    foreach (char c in line)
    {
        Point currentPos = new Point(x, y);
        points.Add(currentPos, c);
        if (isDigit(c))
        {
            currentNumber.AddDigit(getDigit(c), currentPos);
        }
        else
        {
            if (!currentNumber.IsEmpty())
            {
                numbers.Add(currentNumber);
                currentNumber = new Number();
            }
        }
        ++x;
    }
    if (!currentNumber.IsEmpty())
    {
        numbers.Add(currentNumber);
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

bool nextToSymbol(Point p)
{
    return neighbours(p).Any(n => isSymbol(points.GetOrElse(n, '.')));
}

bool isPartNumber(Number n)
{
    return n.GetPosition().Any(nextToSymbol);
}

Console.WriteLine(numbers.Where(isPartNumber).Select(n => n.Value()).Sum());

IEnumerable<Number> adjacentNumbers(Point point)
{
    return numbers.Where(n => n.GetPosition().SelectMany(neighbours).Any(adjacentPos => adjacentPos == point));
}

var gears = points.Where(p => p.Value == '*').Where(p => adjacentNumbers(p.Key).Count() == 2);

Console.WriteLine(gears.Select(g => adjacentNumbers(g.Key).Select(number => number.Value()).Aggregate(1, (a, b) => a * b)).Sum());

public record Point(int x, int y)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
}

public record Number() {
    private int _Value = 0;
    private List<Point> _Position = new List<Point>();
    public void AddDigit(int d, Point position)
    {
        _Position.Add(position);
        _Value *= 10;
        _Value += d;
    }

    public int Value()
    {
        return _Value;
    }

    public IEnumerable<Point> GetPosition()
    {
        return _Position;
    }

    public bool IsEmpty()
    {
        return _Position.Count == 0;
    }
}
