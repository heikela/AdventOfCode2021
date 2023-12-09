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
    return original.Zip(original.Skip(1)).Select(pair => pair.Item2 - pair.Item1);
}

bool isZero(int n)
{
    return n == 0;
}

int extrapolate(IEnumerable<int> original)
{
    if (differences(original).All(isZero))
    {
        return original.Last();
    }
    else
    {
        return original.Last() + extrapolate(differences(original));
    }
}

int extrapolateBack(IEnumerable<int> original)
{
    if (differences(original).All(isZero))
    {
        return original.First();
    }
    else
    {
        return original.First() - extrapolateBack(differences(original));
    }
}

Console.WriteLine("Part2: " + lines.Select(parseLine).Sum(extrapolateBack));
Console.WriteLine(lines.Select(parseLine).Sum(extrapolate));
