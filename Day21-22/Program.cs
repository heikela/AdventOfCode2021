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

if (dependencies.Count(kv => kv.Value.Count > 1) > 0)
{
    Console.WriteLine("Some monkeys are listened to by multiple monkeys. Can't trivially solve problem");
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

Console.WriteLine(monkeys["root"].Solve(0, monkeys, knownValues));

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

    public static bool DependsOnHuman(Monkey monkey, Dictionary<string, Monkey> monkeys)
    {
        if (monkey.Name() == "humn")
        {
            return true;
        }
        return monkey.Dependencies().Any(name => DependsOnHuman(monkeys[name], monkeys));
    }

    public abstract long Solve(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues);

    public abstract long SolveLeft(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues);

    public abstract long SolveRight(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues);
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

    public override long Solve(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        if (Name() == "humn")
        {
            return target;
        }
        throw new NotImplementedException();
    }

    public override long SolveLeft(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        throw new NotImplementedException();
    }

    public override long SolveRight(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        throw new NotImplementedException();
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

    public override long Solve( long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        if (Name() == "root")
        {
            if (Monkey.DependsOnHuman(monkeys[left], monkeys))
            {
                return monkeys[left].Solve(knownValues[right], monkeys, knownValues);
            }
            else
            {
                return monkeys[right].Solve(knownValues[left], monkeys, knownValues);
            }
        }
        if (Monkey.DependsOnHuman(monkeys[left], monkeys))
        {
            return SolveLeft(target, monkeys, knownValues);
        }
        else
        {
            return SolveRight(target, monkeys, knownValues);
        }
    }

    public override long SolveLeft(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        long rightVal = knownValues[right];
        switch (op)
        {
            case "+":
                return monkeys[left].Solve(target - rightVal, monkeys, knownValues);
            case "-":
                return monkeys[left].Solve(target + rightVal, monkeys, knownValues);
            case "*":
                return monkeys[left].Solve(target / rightVal, monkeys, knownValues);
            case "/":
                return monkeys[left].Solve(target * rightVal, monkeys, knownValues);
        }
        throw new ArgumentException($"Unknown operation {op}");
    }

    public override long SolveRight(long target, Dictionary<string, Monkey> monkeys, Dictionary<string, long> knownValues)
    {
        long leftVal = knownValues[left];
        switch (op)
        {
            case "+":
                return monkeys[right].Solve(target - leftVal, monkeys, knownValues);
            case "-":
                return monkeys[right].Solve(leftVal - target, monkeys, knownValues);
            case "*":
                return monkeys[right].Solve(target / leftVal, monkeys, knownValues);
            case "/":
                return monkeys[right].Solve(target / leftVal, monkeys, knownValues);
        }
        throw new ArgumentException($"Unknown operation {op}");
    }


}
