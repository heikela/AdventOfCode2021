using Common;
using Priority_Queue;


//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

ConcreteGraph<string> allNodes = new ConcreteGraph<string>();
Dictionary<string, int> workingValves = new Dictionary<string, int>();
ConcreteWeightedGraph<string> interestingNodes = new ConcreteWeightedGraph<string>();
Dictionary<(string, string), int> interestingDistances = new Dictionary<(string, string), int>();

foreach (var line in input)
{
    string[] parts = line.Split(' ');
    string node = parts[1];
    string flowDesc = parts[4];
    int flow = int.Parse(flowDesc.Substring(5, flowDesc.Length - 6));

    if (flow > 0)
    {
        workingValves[node] = flow;
    }

    var destinations = parts.Skip(9);

    foreach (var dest in destinations)
    {
        string destName = dest.Substring(0, 2);
        allNodes.AddEdge(node, destName);
        allNodes.AddEdge(destName, node);
    }
}

List<string> interesting = workingValves.Keys.ToList();
interesting.Add("AA");

for (int i = 0; i < interesting.Count; i++)
{
    for (int j = i + 1; j < interesting.Count; j++)
    {
        int dist = allNodes.ShortestPathTo(interesting[i], interesting[j]).GetLength();
        interestingNodes.AddEdge(interesting[i], interesting[j], dist);
        interestingNodes.AddEdge(interesting[j], interesting[i], dist);
        interestingDistances.Add((interesting[i], interesting[j]), dist);
        interestingDistances.Add((interesting[j], interesting[i]), dist);
    }
}

interesting.Remove("AA");

SimplePriorityQueue<State> options = new SimplePriorityQueue<State>();

options.Enqueue(new State(interesting, "AA", 30, 0, 0, 0), 0);

State CalculateMove(State current, string chosen)
{
    int dist = interestingDistances[(current.current, chosen)];
    int timeSpent = dist + 1;
    int newTimeLeft = current.timeLeft - timeSpent;
    int newReleasing = current.releasing + workingValves[chosen];
    int released = current.released + timeSpent * current.releasing;
    int forecast = released + newTimeLeft * newReleasing;
    List<string> newCandidates = new List<string>(current.candidates);
    newCandidates.Remove(chosen);
    return new State(newCandidates, chosen, newTimeLeft, newReleasing, released, forecast);
}

int best = 0;
while (options.Count > 0)
{
    State currentState = options.Dequeue();

    IEnumerable<State> possibleStates = currentState.candidates.Select(x => CalculateMove(currentState, x)).Where(s => s.timeLeft >= 0);
    foreach (var move in possibleStates)
    {
        options.Enqueue(move, -move.forecast);
        if (move.forecast > best)
        {
            best = move.forecast;
            Console.WriteLine($"We can do {best}");
        }
    }
}

public record State(List<string> candidates, string current, int timeLeft, int releasing, int released, int forecast);

