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
    return processAdjacent(original, (a, b) => b - a);
}

IEnumerable<T> processAdjacent<T>(IEnumerable<T> sequence, Func<T, T, T> selector)
{
    IEnumerator<T> iterator = sequence.GetEnumerator();
    if (!iterator.MoveNext())
    {
        yield break;
    }
    T prev = iterator.Current;
    while (iterator.MoveNext())
    {
        T current = iterator.Current;
        yield return selector(prev, current);
        prev = current;
    }
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
