using Common;

ConcreteGraph<string> cave = new ConcreteGraph<string>();

var input = File.ReadAllLines("input12.txt").Select(l => l.Split('-').ToArray());

foreach (var line in input)
{
    cave.AddEdge(line[0], line[1]);
    cave.AddEdge(line[1], line[0]);
}

Func<string, Boolean> isSmall = x => x.ToLower() == x;

Func<string, List<string>, Boolean> canVisit = (x, previous) =>
{
    if (x == "start") return false;
    if (!isSmall(x)) return true;
    if (!previous.Contains(x)) return true;
    for (int i = 1; i < previous.Count; i++)
    {
        if (isSmall(previous[i]) && previous.Take(i - 1).Contains(previous[i]))
        {
            return false;
        }
    }
    return true;
};

//Dictionary<T, T> visited = new Dictionary<T, T>();
Stack<List<string> > frontier = new Stack<List<string>>();
frontier.Push(new List<string>() { "start" });
long validPaths = 0;
while (frontier.Any())
{
    List<string> current = frontier.Pop();
    foreach (string next in cave.GetNeighbours(current[0]))
    {
        // part 1
        // if (isSmall(next) && current.Contains(next)) continue;

        // part 2
        if (!canVisit(next, current)) continue;
        // end part 2 specific

        if (next == "end")
        {
            validPaths++;
            continue;
        }
        frontier.Push(new List<string>() { next }.Concat(current).ToList());
    }
}

Console.WriteLine($"{validPaths}");


