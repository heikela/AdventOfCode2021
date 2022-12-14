using Common;

var input = File.ReadAllLines("../../../input.txt");
//var input = File.ReadAllLines("../../../testInput.txt");

List<Monkey> monkeys = new List<Monkey>();

foreach (var monkeyDescription in input.Paragraphs())
{
    monkeys.Add(Monkey.Parse(monkeyDescription));
}

for (int round = 0; round < 20; round++) {
    for (int monkey = 0; monkey < monkeys.Count; monkey++) {
        int dest;
        do
        {
            (dest, long item) = monkeys[monkey].InspectNextItem();
            if (dest >= 0)
            {
                monkeys[dest].AddItem(item);
            }
        } while (dest >= 0);
    }
}

long[] mostActive = monkeys.Select(m => m.Activity).OrderByDescending(x => x).Take(2).ToArray();

long result = mostActive[0] * mostActive[1];
Console.WriteLine(result);

monkeys = new List<Monkey>();

foreach (var monkeyDescription in input.Paragraphs())
{
    monkeys.Add(Monkey.Parse(monkeyDescription));
}

int commonDivisor = monkeys.Select(m => m.Divisor).Aggregate((a, b) => a * b);

for (int round = 0; round < 10000; round++)
{
    if (round == 1 || round == 20 || round == 1000)
    {
        Console.WriteLine($"== After round {round} ==");
        int i = 0;
        foreach(Monkey monkey in monkeys)
        {
            Console.WriteLine($"Monkey {i} inspected items {monkey.Activity} times.");
            ++i;
        }
    }
    for (int monkey = 0; monkey < monkeys.Count; monkey++)
    {
        int dest, dest2;
        do
        {
            (dest, long item) = monkeys[monkey].InspectNextItem2(commonDivisor);
            if (dest >= 0)
            {
                monkeys[dest].AddItem(item);
            }
        } while (dest >= 0);
    }
}

mostActive = monkeys.Select(m => m.Activity).OrderByDescending(x => x).Take(2).ToArray();

result = mostActive[0] * mostActive[1];
Console.WriteLine(result);

class Monkey
{
    public static Monkey Parse(IEnumerable<string> input)
    {
        Monkey monkey = new Monkey();
        string[] lines = input.ToArray();
        for (int i = 0; i < 6; ++i)
        {
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    {
                        var itemPart = lines[i].Substring("  Starting items: ".Length);
                        var items = itemPart.Split(", ");
                        monkey.Items = new Queue<long>(items.Select(s => long.Parse(s)));
                    }
                    break;
                case 2:
                    {
                        var opPart = lines[i].Substring("  Operation: new = ".Length);
                        if (opPart == "old * old")
                        {
                            monkey.Op = x => x * x;
                        }
                        else if (opPart.StartsWith("old + ")) {
                            int c = int.Parse(opPart.Split(' ').Last());
                            monkey.Op = x => x + c;
                        }
                        else if (opPart.StartsWith("old * "))
                        {
                            int c = int.Parse(opPart.Split(' ').Last());
                            monkey.Op = x => x * c;
                        }
                    }
                    break;
                case 3:
                    monkey.Divisor = int.Parse(lines[i].Split(' ').Last());
                    break;
                case 4:
                    monkey.TrueDest = int.Parse(lines[i].Split(' ').Last());
                    break;
                case 5:
                    monkey.FalseDest = int.Parse(lines[i].Split(' ').Last());
                    break;
            }
        }
        return monkey;
    }

    public (int dest, long item) InspectNextItem()
    {
        if (Items.Count == 0)
        {
            return (-1, -1);
        }
        Activity++;
        long item = Items.Dequeue();
        item = Op(item);
        item = item / 3;
        int dest = item % Divisor == 0 ? TrueDest : FalseDest;
        return (dest, item);
    }

    public (int dest, long item) InspectNextItem2(int commonDivisor)
    {
        if (Items.Count == 0)
        {
            return (-1, -1);
        }
        Activity++;
        long item = Items.Dequeue();
        item = Op(item);
        item = item % commonDivisor;
        int dest = item % Divisor == 0 ? TrueDest : FalseDest;

        return (dest, item);
    }

    public void AddItem(long item)
    {
        Items.Enqueue(item);
    }

    public Queue<long> Items = new Queue<long>();
    private Func<long, long> Op = x => { throw new Exception("Op not initialized"); };
    public int Divisor = 0;
    private int TrueDest = 0;
    private int FalseDest = 0;
    public long Activity = 0;
}
