using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

// First, figure out how to interpret the characters in the input

const char EMPTY = '.';

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
    return c != EMPTY && !isDigit(c);
}

// Parse the input into lists of more manageable elements

List<Number> numbers = new List<Number>();
List<Symbol> symbols = new List<Symbol>();

int y = 0;
foreach (string line in lines)
{
    int x = 0;
    Number currentNumber = new Number();
    foreach (char c in line)
    {
        Point currentPos = new Point(x, y);
        if (isDigit(c))
        {
            currentNumber.AddDigit(getDigit(c), currentPos);
        }
        else
        {
            // Save number if we've just finished one
            if (!currentNumber.IsEmpty())
            {
                numbers.Add(currentNumber);
                currentNumber = new Number();
            }
            // Save a symbol if we've found one
            if (isSymbol(c))
            {
                symbols.Add(new Symbol(c, currentPos));
            }
        }
        ++x;
    }
    // Save a number at the end of the line if needed
    if (!currentNumber.IsEmpty())
    {
        numbers.Add(currentNumber);
    }
    ++y;
}

List<Element> elements = numbers.Union<Element>(symbols).ToList();

// Maintain a lookup of elements by position

Dictionary<Point, Element> elementsByPos = elements.SelectMany(el => el.GetPositions().Select(p => new KeyValuePair<Point, Element>(p, el))).ToDictionary();

// Define adjacency

Point zero = new Point(0, 0);
List<Point> adjacent = Enumerable.Range(-1, 3).SelectMany(dy => Enumerable.Range(-1, 3).Select(dx => new Point(dx, dy))).Where(p => p != zero).ToList();

IEnumerable<Point> adjacentPoints(Point point)
{
    return adjacent.Select(d => d + point);
}

IEnumerable<Point> AdjacentPositions(Element el)
{
    return el.GetPositions().SelectMany(adjacentPoints).Where(p => !el.GetPositions().Contains(p));
}

IEnumerable<Element> neighbours(Element element)
{
    return AdjacentPositions(element).SelectMany(p =>
    {
        if (elementsByPos.ContainsKey(p))
        {
            return new Element[] { elementsByPos[p] };
        }
        else
        {
            return new Element[] {};
        }
    }).ToHashSet();
}

// Define part numbers based on adjacency

bool isPartNumber(Number n)
{
    return neighbours(n).Any(el => el.IsSymbol());
}

// Solve part 1

Console.WriteLine($"Sum of part numbers for part 1: {numbers.Where(isPartNumber).Select(n => n.Value()).Sum()}");

// Find gears

var gears = symbols.Where(s => s.IsSymbol('*')).Where(s => neighbours(s).Count(n => n.IsNumber()) == 2);

// Solve part 2

var gearRatioSum = gears.Select(g => neighbours(g).Where(n => n.IsNumber()).Select(number => number.Value()).Aggregate(1, (a, b) => a * b)).Sum();
Console.WriteLine($"Sum of gear ratios for part 2: {gearRatioSum}");

// Classes to encapsulate some of the data and operations
public record Point(int x, int y)
{
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
}

public class Element
{
    protected List<Point> Positions = new List<Point>();

    public virtual bool IsSymbol() { return false; }
    public virtual bool IsSymbol(char symbol) { return false; }
    public virtual bool IsNumber() { return false; }
    public virtual int Value() { throw new NotImplementedException("Trying to call Value on an element that is not a number"); }
    public IEnumerable<Point> GetPositions() { return Positions; }
}

public class Number : Element {
    private int _Value = 0;
    public void AddDigit(int d, Point position)
    {
        Positions.Add(position);
        _Value *= 10;
        _Value += d;
    }

    public override 
        int Value()
    {
        return _Value;
    }

    public override bool IsNumber()
    {
        return true;
    }

    public bool IsEmpty()
    {
        return Positions.Count == 0;
    }
}

public class Symbol : Element {
    private readonly char TheSymbol;
    public Symbol(char symbol, Point position)
    {
        Positions = new List<Point>() { position };
        TheSymbol = symbol;
    }
    public override bool IsSymbol()
    {
        return true;
    }
    public override bool IsSymbol(char symbol)
    {
        return symbol == TheSymbol;
    }
}
