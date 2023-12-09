using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

IEnumerable<int> parseLine(string line)
{
    return line.Split(' ').Select(int.Parse);
}

IEnumerable<int> differences(IEnumerable<int> original)
{
    return original.ProcessAdjacent((a, b) => b - a);
}

bool isZero(int n)
{
    return n == 0;
}

int extrapolate(IEnumerable<int> original)
{
    IEnumerable<int> diff = differences(original);
    if (diff.All(isZero))
    {
        return original.Last();
    }
    else
    {
        return original.Last() + extrapolate(diff);
    }
}

Console.WriteLine($"Part 1: {lines.Select(parseLine).Sum(extrapolate)}");
Console.WriteLine($"Part 2: {lines.Select(parseLine).Select(numbers => numbers.Reverse()).Sum(extrapolate)}");
