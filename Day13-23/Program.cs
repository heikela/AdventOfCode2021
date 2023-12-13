using Common;

var fileName = "../../../input.txt";
//var fileName = "../../../testInput.txt";

var patterns = File.ReadAllLines(fileName).Paragraphs();

Console.WriteLine($"Part 1 : {patterns.Select(Pattern.Parse).Sum(p => p.ReflectionValue())}");

public class Pattern
{
    private int W;
    private int H;
    private Dictionary<Point, char> Scan;

    private Pattern()
    {
        Scan = new Dictionary<Point, char>();
    }

    public static Pattern Parse(IEnumerable<string> lines)
    {
        Pattern result = new Pattern();
        result.H = lines.Count();
        result.W = lines.First().Length;
        int y = 0;
        foreach (var line in lines)
        {
            for (int x = 0; x < result.W; x++)
            {
                result.Scan[new Point(x, y)] = line[x];
            }
            ++y;
        }
        return result;
    }

    private string GetRow(int y)
    {
        return string.Join("", Enumerable.Range(0, W).Select(x => Scan[new Point(x, y)]));
    }

    private string GetColumn(int x)
    {
        return string.Join("", Enumerable.Range(0, H).Select(y => Scan[new Point(x, y)]));
    }

    public int ReflectionValue()
    {
        for (int x = 1; x < W; x++)
        {
            bool isMirror = true;
            for (int d = 0; x - 1 - d >= 0 && x + d < W; ++d)
            {
                if (GetColumn(x - 1 - d) != GetColumn(x + d))
                {
                    isMirror = false;
                    break;
                }
            }
            if (isMirror)
            {
                return x;
            }
        }

        for (int y = 1; y < H; y++)
        {
            bool isMirror = true;
            for (int d = 0; y - 1 - d >= 0 && y + d < H; ++d)
            {
                if (GetRow(y - 1 - d) != GetRow(y + d))
                {
                    isMirror = false;
                    break;
                }
            }
            if (isMirror)
            {
                return 100 * y;
            }
        }
        throw new Exception("No mirror found");
    }
}

public record Point(int X, int Y)
{

}

