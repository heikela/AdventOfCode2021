using Common;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput1.txt";

Dictionary<string, Module> modules = new Dictionary<string, Module>();
Dictionary<string, List<string>> reverseConnections = new Dictionary<string, List<string>>();

foreach (var line in File.ReadAllLines(fileName))
{
    string[] parts = line.Split("->", StringSplitOptions.TrimEntries);
    string id = parts[0].Substring(1);
    string[] destinations = parts[1].Split(',', StringSplitOptions.TrimEntries);
    foreach (var dest in destinations)
    {
        if (!reverseConnections.ContainsKey(dest))
        {
            reverseConnections[dest] = new List<string>();
        }
        reverseConnections[dest].Add(id);
    }
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

for (int round = 0; round < 1000; round++)
{
    pulses.Enqueue(new Pulse(false, "broadcaster", "in"));
    while (pulses.Count > 0)
    {
        Pulse pulse = pulses.Dequeue();
        if (pulse.High)
        {
            highCount++;
        }
        else
        {
            lowCount++;
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

