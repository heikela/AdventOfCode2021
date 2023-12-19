using Common;
using System.Data;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

var sections = File.ReadAllLines(fileName).Paragraphs();

var parts = sections.Skip(1).First().Select(line =>
{
    int[] values = line.Substring(1, line.Length - 2).Split(',').Select(varSection => int.Parse(varSection.Split('=')[1])).ToArray();
    return new Part(values[0], values[1], values[2], values[3]);
}).ToArray();

var workflows = sections.First().Select(line =>
{
    Char[] splitChars = new Char[] { '{', ',', '}' };
    var topSplit = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
    string id = topSplit[0];
    string defaultDest = topSplit.Last();
    List<Rule> rules = new List<Rule>();
    for (int i = 1; i < topSplit.Length - 1; i++)
    {
        string ruleDef = topSplit[i];
        var ruleParts = ruleDef.Split(':');
        string var = ruleParts[0].Substring(0, 1);
        string op = ruleParts[0].Substring(1, 1);
        int threshold = int.Parse(ruleParts[0].Substring(2));
        string dest = ruleParts[1];
        var rule = new Rule(var, op, threshold, dest);
        rules.Add(rule);
    }
    return new WorkFlow(id, rules, defaultDest);
}).ToArray();

Dictionary<string, WorkFlow> workFlowById = workflows.ToDictionary(workflow => workflow.Id);

long sum = 0;
foreach (var part in parts)
{
    string current = "in";
    while (current != "A" && current != "R")
    {
        var workflow = workFlowById[current];
        current = workflow.GetDestination(part);
    }
    if (current == "A")
    {
        sum += part.Value();
    }
}
Console.WriteLine($"Part 1: {sum}");

public record Part(int X, int M, int A, int S)
{
    public int Value()
    {
        return X + M + A + S;
    }
}

public record Rule(string Var, String Op, int Threshold, string Dest)
{
    public bool Satisfies(Part part)
    {
        int value = Var switch
        {
            "x" => part.X,
            "m" => part.M,
            "a" => part.A,
            "s" => part.S,
            _ => throw new Exception("Invalid variable")
        };

        switch (Op)
        {
            case ">":
                return value > Threshold;
            case "<":
                return value < Threshold;
            default:
                throw new Exception("Invalid operator");
        }
    }
}

public record WorkFlow(string Id, List<Rule> Rules, string DefaultDest)
{
    public string GetDestination(Part part)
    {
        foreach (var rule in Rules)
        {
            if (rule.Satisfies(part))
            {
                return rule.Dest;
            }
        }

        return DefaultDest;
    }
}
