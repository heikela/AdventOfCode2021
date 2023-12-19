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

Queue<(string, PartRange)> ranges = new Queue<(string, PartRange)>();
ranges.Enqueue(("in", new PartRange(new Range(1, 4000), new Range(1, 4000), new Range(1, 4000), new Range(1, 4000))));

long acceptedCount = 0;
while (ranges.Count > 0)
{
    (string current, PartRange range) = ranges.Dequeue();
    WorkFlow workflow = workFlowById[current];
    foreach (var (next, nextRange) in range.RangesFromWorkFlow(workflow))
    {
        if (next == "A")
        {
            acceptedCount += nextRange.Count();
        }
        else if (next == "R")
        {
            continue;
        }
        else
        {
            ranges.Enqueue((next, nextRange));
        }
    }
}

Console.WriteLine($"Part 2: {acceptedCount}");

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

public record Range(int Min, int Max)
{
    public long Count()
    {
        return Max - Min + 1;
    }

    public bool IsEmpty()
    {
        return Min > Max;
    }

    public (Range, Range) SplitByLessThan(int threshold)
    {
        if (Min > threshold)
        {
            return (new Range(0, -1), this);
        }
        else if (Max < threshold)
        {
            return (this, new Range(0, -1));
        }
        else
        {
            return (new Range(Min, threshold - 1), new Range(threshold, Max));
        }
    }

    public (Range, Range) SplitByGreaterThan(int threshold)
    {
        if (Min > threshold)
        {
            return (this, new Range(0, -1));
        }
        else if (Max < threshold)
        {
            return (new Range(0, -1), this);
        }
        else
        {
            return (new Range(threshold + 1, Max), new Range(Min, threshold));
        }
    }

}

public record PartRange(Range XRange, Range MRange, Range ARange, Range SRange)
{
    public long Count()
    {
        return XRange.Count() * MRange.Count() * ARange.Count() * SRange.Count();
    }

    private (PartRange, PartRange) SplitByRule(Rule rule)
    {
        Range varRange = rule.Var switch
        {
            "x" => XRange,
            "m" => MRange,
            "a" => ARange,
            "s" => SRange,
            _ => throw new Exception("Invalid variable")
        };
        (Range matches, Range doesNotMatch) = (rule.Op) switch
        {
            ">" => varRange.SplitByGreaterThan(rule.Threshold),
            "<" => varRange.SplitByLessThan(rule.Threshold),
            _ => throw new Exception("Invalid operator")
        };
        switch (rule.Var)
        {
            case "x":
                return (new PartRange(matches, MRange, ARange, SRange), new PartRange(doesNotMatch, MRange, ARange, SRange));
            case "m":
                return (new PartRange(XRange, matches, ARange, SRange), new PartRange(XRange, doesNotMatch, ARange, SRange));
            case "a":
                return (new PartRange(XRange, MRange, matches, SRange), new PartRange(XRange, MRange, doesNotMatch, SRange));
            case "s":
                return (new PartRange(XRange, MRange, ARange, matches), new PartRange(XRange, MRange, ARange, doesNotMatch));
            default:
                throw new Exception("Invalid variable");
        }
    }

    public bool IsEmpty()
    {
        return XRange.IsEmpty() || MRange.IsEmpty() || ARange.IsEmpty() || SRange.IsEmpty();
    }

    public IEnumerable<(string, PartRange)> RangesFromWorkFlow(WorkFlow workFlow)
    {
        PartRange remaining = this;
        foreach (Rule rule in workFlow.Rules)
        {
            (PartRange matches, PartRange doesNotMatch) = remaining.SplitByRule(rule);
            if (!matches.IsEmpty())
            {
                yield return (rule.Dest, matches);
            }
            remaining = doesNotMatch;
        }
        if (!remaining.IsEmpty())
        {
            yield return (workFlow.DefaultDest, remaining);
        }
    }
}
