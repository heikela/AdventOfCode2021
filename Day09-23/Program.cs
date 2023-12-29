using Common;

string[] lines = new AoCUtil().GetInput(2023, 9);
//string[] lines = new AoCUtil().GetTestBlock(2023, 9, 0);

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
