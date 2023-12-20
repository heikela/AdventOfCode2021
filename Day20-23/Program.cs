using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput1.txt";

Dictionary<string, Module> modules = new Dictionary<string, Module>();
Dictionary<string, List<string>> reverseConnections = new Dictionary<string, List<string>>();
Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();

foreach (var line in File.ReadAllLines(fileName))
{
    string[] parts = line.Split("->", StringSplitOptions.TrimEntries);
    string id = parts[0].Substring(1);
    string[] destinations = parts[1].Split(',', StringSplitOptions.TrimEntries);
    if (parts[0] == "broadcaster")
    {
        id = "broadcaster";
        modules[id] = new Broadcast(id, destinations.ToList());
    }
    if (parts[0][0] == '%')
    {
        modules[id] = new FlipFlop(id, destinations.ToList());
    }
    if (parts[0][0] == '&')
    {
        modules[id] = new Conjunction(id, destinations.ToList());
    }
    connections.Add(id, destinations.ToList());
    foreach (var dest in destinations)
    {
        if (!reverseConnections.ContainsKey(dest))
        {
            reverseConnections[dest] = new List<string>();
        }
        reverseConnections[dest].Add(id);
    }
}

HashSet<string> visited = new HashSet<string>();
Dictionary<string, string> roots = new Dictionary<string, string>();
Stack<string> L = new Stack<string>();

void visit(string id)
{
    if (!visited.Contains(id))
    {
        visited.Add(id);
        foreach (var dest in connections.GetOrElse(id, new List<string>()))
        {
            visit(dest);
        }
        L.Push(id);
    }
}

foreach (var kv in connections)
{
    visit(kv.Key);
}

void assign(string id, string root)
{
    if (!roots.ContainsKey(id))
    {
        roots[id] = root;
        foreach (var dest in reverseConnections.GetOrElse(id, new List<string>()))
        {
            assign(dest, root);
        }
    }
}

while (L.Count > 0)
{
    string id = L.Pop();
    assign(id, id);
}

foreach (var group in roots.GroupBy(kv => kv.Value))
{
    Console.WriteLine($"Group {group.Key}: {string.Join(", ", group.Select(kv => kv.Key))}");
}

foreach (var kv in reverseConnections)
{
    if (modules.ContainsKey(kv.Key))
    {
        modules[kv.Key].Init(kv.Value);
    }
}

Queue<Pulse> pulses = new Queue<Pulse>();
int lowCount = 0;
int highCount = 0;
bool done = false;
int round = 1;
List<string> interesting = new List<string>() { "rd", "bt", "fv", "pr" };

while (!done)
{
    pulses.Enqueue(new Pulse(false, "broadcaster", "in"));
    while (pulses.Count > 0)
    {
        Pulse pulse = pulses.Dequeue();
        if (pulse.High)
        {
            highCount++;
            if (interesting.Contains(pulse.From))
            {
                Console.WriteLine($"{pulse.From} sends high at {round}");
            }
        }
        else
        {
            lowCount++;
            if (pulse.Destination == "rx")
            {
                Console.WriteLine($"Part 2: {round}");
                done = true;
            }
        }
        string type = pulse.High ? "high" : "low";
        //    Console.WriteLine($"{pulse.From} -{type}-> {pulse.Destination}");
        Module module = modules.GetOrElse(pulse.Destination, null);
        if (module != null)
        {
            foreach (var newPulse in modules[pulse.Destination].Process(pulse))
            {
                pulses.Enqueue(newPulse);
            }
        }
    }
    round++;
}

long result = (long)lowCount * (long)highCount;
Console.WriteLine($"Part 1 intermediate: {lowCount}, {highCount}");
Console.WriteLine($"Part 1: {result}");


public record Pulse (bool High, string Destination, string From);

public abstract class Module
{
    protected string Id;
    protected List<string> Destinations;
    protected Module(string id, List<string> destinations)
    {
        Id = id;
        Destinations = destinations;
    }
    public abstract IEnumerable<Pulse> Process(Pulse pulse);
    public virtual void Init(IEnumerable<string> sources)
    {
    }

}

public class  FlipFlop : Module
{
    private bool _state;
    public FlipFlop(string id, List<string> destinations) : base(id, destinations)
    {
        _state = false;
    }
    public override IEnumerable<Pulse> Process(Pulse pulse)
    {
        if (!pulse.High)
        {
            _state = !_state;
            foreach(var dest in Destinations)
            {
                yield return new Pulse(_state, dest, Id);
            }
        }
    }
}

public class Conjunction : Module
{
    private Dictionary<string, bool> _oldSignals;
    public Conjunction(string id, List<string> destinations) : base(id, destinations)
    {
        _oldSignals = new Dictionary<string, bool>();
    }
    public override IEnumerable<Pulse> Process(Pulse pulse)
    {
        _oldSignals[pulse.From] = pulse.High;
        bool allHigh = _oldSignals.Values.All(v => v);
        foreach (var dest in Destinations)
        {
            yield return new Pulse(!allHigh, dest, Id);
        }
    }

    public override void Init(IEnumerable<string> sources)
    {
        foreach (var source in sources)
        {
            _oldSignals[source] = false;
        }
    }
}

public class Broadcast : Module
{
    public Broadcast(string id, List<string> destinations) : base(id, destinations)
    {
    }
    public override IEnumerable<Pulse> Process(Pulse pulse)
    {
        foreach (var dest in Destinations)
        {
            yield return new Pulse(pulse.High, dest, Id);
        }
    }
}

