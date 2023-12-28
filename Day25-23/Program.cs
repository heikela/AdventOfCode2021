using Common;

string[] lines = new AoCUtil().GetInput(2023, 25);
//string[] lines = new AoCUtil().GetTestBlock(2023, 25, 1);

ConcreteGraph<string> graph = new ConcreteGraph<string>();

foreach (string line in lines)
{
    char[] separators = { ' ', ':' };
    string[] parts = line.Split(separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    foreach (string dest in parts.Skip(1))
    {
        graph.AddEdge(parts[0], dest);
        graph.AddEdge(dest, parts[0]);
    }
}

List<List<string>> halves = graph.KargerMinimumCut(3).ToList();

Console.WriteLine($"Min cut: {halves[0].Count * halves[1].Count}");
