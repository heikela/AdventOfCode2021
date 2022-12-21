//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();
Dictionary<string, long> knownValues = new Dictionary<string, long>();
Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();
HashSet<Monkey> readyToCalculate = new HashSet<Monkey>();

foreach (var line in input)
{
    Monkey monkey = Monkey.Parse(line);
    monkeys.Add(monkey.Name(), monkey);
    if (!dependencies.ContainsKey(monkey.Name()))
    {
        dependencies.Add(monkey.Name(), new List<string>());
    }
    foreach (var dependency in monkey.Dependencies())
    {
        if (!dependencies.ContainsKey(dependency))
        {
            dependencies.Add(dependency, new List<string>());
        }
        dependencies[dependency].Add(monkey.Name());
    }
    if (monkey.ValueKnownAtStart())
    {
        readyToCalculate.Add(monkey);
    }
}

while (readyToCalculate.Count > 0)
{
    Monkey current = readyToCalculate.First();
    readyToCalculate.Remove(current);
    long value = current.CalculateValue(knownValues);
    knownValues.Add(current.Name(), value);
    foreach (string dependency in dependencies[current.Name()])
    {
        Monkey possiblyReadyMonkey = monkeys[dependency];
        if (possiblyReadyMonkey.Dependencies().All(s => knownValues.ContainsKey(s)))
        {
            readyToCalculate.Add(possiblyReadyMonkey);
        }
    }
    if (current.Name() == "root")
    {
        Console.WriteLine($"Root monkey yells {value}");
    }
}


public abstract record Monkey()
{
    public static Monkey Parse(string line)
    {
        /*
            root: pppw + sjmn
        dbpl: 5
        */
        string[] parts = line.Split(' ');
        if (parts.Length == 2)
        {
            return new StaticMonkey(parts[0].Substring(0, parts[0].Length - 1), long.Parse(parts[1]));
        }
        else
        {
            return new OpMonkey(parts[0].Substring(0, parts[0].Length - 1), parts[2], parts[1], parts[3]);
        }

    }

    public abstract string Name();

    public abstract IEnumerable<string> Dependencies();

    public abstract long CalculateValue(Dictionary<string, long> knownValues);

    public abstract bool ValueKnownAtStart();
}

public record StaticMonkey(string name, long value) : Monkey
{
    public override string Name()
    {
        return name;
    }

    public override IEnumerable<string> Dependencies()
    {
        yield break;
    }

    public override long CalculateValue(Dictionary<string, long> knownValues)
    {
        return value;
    }

    public override bool ValueKnownAtStart()
    {
        return true;
    }

}

public record OpMonkey(string name, string op, string left, string right) : Monkey
{
    public override string Name()
    {
        return name;
    }

    public override IEnumerable<string> Dependencies()
    {
        yield return left;
        yield return right;
        yield break;
    }

    public override long CalculateValue(Dictionary<string, long> knownValues)
    {
        long leftVal = knownValues[left];
        long rightVal = knownValues[right];
        switch (op)
        {
            case "+":
                return leftVal + rightVal;
            case "-":
                return leftVal - rightVal;
            case "*":
                return leftVal * rightVal;
            case "/":
                return leftVal / rightVal;
        }
        throw new Exception($"Unknown operation {op}");
    }

    public override bool ValueKnownAtStart()
    {
        return false;
    }
}
