using Common;
using Priority_Queue;


//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

ConcreteGraph<string> allNodes = new ConcreteGraph<string>();
Dictionary<string, int> workingValves = new Dictionary<string, int>();
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
workingValves["AA"] = 0;
int unreleased = workingValves.Values.Sum();
workingValves["DONE"] = 0;


for (int i = 0; i < interesting.Count; i++)
{
    for (int j = i + 1; j < interesting.Count; j++)
    {
        int dist = allNodes.ShortestPathTo(interesting[i], interesting[j]).GetLength();
        interestingDistances.Add((interesting[i], interesting[j]), dist);
        interestingDistances.Add((interesting[j], interesting[i]), dist);
    }
}

interesting.Remove("AA");

SimplePriorityQueue<State> options = new SimplePriorityQueue<State>();

options.Enqueue(new State(interesting.ToList(), "AA", 30, 0, 0, 0), 0);

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

ActorState CalculateActorMove(ActorState current, string chosen)
{
    return new ActorState(chosen, current.nextMoveTimeLeft - interestingDistances[(current.current, chosen)] - 1);
}

ActorState CalculateActorNullMove(ActorState current)
{
    return new ActorState("DONE", current.nextMoveTimeLeft - 1);
}

IEnumerable<StateWithElephant> CalculateElephantMoves(StateWithElephant current)
{
    bool elephantToMove = current.elephant.nextMoveTimeLeft == current.timeLeft;
    bool meToMove = current.me.nextMoveTimeLeft == current.timeLeft;

    if (elephantToMove && meToMove && current.candidates.Count == 0)
    {
        yield break;
    }

    List<ActorState> possibleElephantStates = new List<ActorState>();
    if (elephantToMove)
    {
        possibleElephantStates = current.candidates.Select(d => CalculateActorMove(current.elephant, d)).ToList();
        if (possibleElephantStates.Count == 0)
        {
            possibleElephantStates.Add(CalculateActorNullMove(current.elephant));
        }
    }
    else
    {
        possibleElephantStates.Add(current.elephant);
    }
    List<(ActorState, ActorState)> possibleMeAndElephant = new List<(ActorState, ActorState)>();
    if (meToMove)
    {
        foreach (ActorState e in possibleElephantStates)
        {
            List<string> available = current.candidates.ToList();
            available.Remove(e.current);
            foreach (string d in available)
            {
                possibleMeAndElephant.Add((CalculateActorMove(current.me, d), e));
            }
            if (available.Count == 0)
            {
                possibleMeAndElephant.Add((CalculateActorNullMove(current.me), e));
            }
        }
    }
    else
    {
        possibleMeAndElephant = possibleElephantStates.Select(e => (current.me, e)).ToList();
    }

    foreach (var actors in possibleMeAndElephant)
    {
        ActorState newMe = actors.Item1;
        ActorState newElephant = actors.Item2;

        int newTimeLeft = Math.Max(newMe.nextMoveTimeLeft, newElephant.nextMoveTimeLeft);
        int timeSpent = current.timeLeft - newTimeLeft;

        int addedRelease = 0;
        if (newMe.nextMoveTimeLeft == newTimeLeft)
        {
            addedRelease += workingValves[newMe.current];
        }
        if (newElephant.nextMoveTimeLeft == newTimeLeft)
        {
            addedRelease += workingValves[newElephant.current];
        }
        int newReleasing = current.releasing + addedRelease;
        int released = current.released + timeSpent * current.releasing;
        int cost = (unreleased - current.releasing) * timeSpent;
//        int forecast = released + newTimeLeft * newReleasing;
        List<string> newCandidates = current.candidates.ToList();
        newCandidates.Remove(newMe.current);
        newCandidates.Remove(newElephant.current);
//        string newHistory = current.history;

/*
        if (newMe.current != current.me.current)
        {
            newHistory += $", at time {26 - newMe.nextMoveTimeLeft} I open valve {newMe.current}";
        }
        if (newElephant.current != current.elephant.current)
        {
            newHistory += $", at time {26 - newElephant.nextMoveTimeLeft} the elephant opens valve {newElephant.current}";
        }
*/
        if (newTimeLeft >= 0)
        {
            yield return new StateWithElephant(newCandidates, newMe, newElephant, newTimeLeft, newReleasing, released, current.loss + cost /*, forecast, newHistory */);
        }
    }
}

int best = 0;
/*
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
*/

best = 0;
int bestForecast = 0;

SimplePriorityQueue<StateWithElephant> elephantOptions = new SimplePriorityQueue<StateWithElephant>();

elephantOptions.Enqueue(new StateWithElephant(interesting.ToList(), new ActorState("AA", 26), new ActorState("AA", 26), 26, 0, 0, 0 /*, ""*/), 0);

while (elephantOptions.Count > 0)
{
    StateWithElephant currentState = elephantOptions.Dequeue();

    if (currentState.released > best)
    {
        best = currentState.released;
        Console.WriteLine($"We can do {best}");
    }
    /*
    if (currentState.candidates.Count == 0)
    {
        Console.WriteLine($"We can open all valves with {currentState.timeLeft} time left");
    }
    */
    List<StateWithElephant> possibleStates = CalculateElephantMoves(currentState).ToList();
    if (possibleStates.Count == 0)
    {
        int forecast = currentState.released + currentState.releasing * currentState.timeLeft;
        if (forecast > bestForecast)
        {
            bestForecast = forecast;
            Console.WriteLine($"Forecasting {forecast}");
        }
    }

    foreach (var move in possibleStates)
    {
        if (move.released + move.timeLeft * unreleased > bestForecast)
        {
            elephantOptions.Enqueue(move, move.loss);
        }
    }
}

public record State(List<string> candidates, string current, int timeLeft, int releasing, int released, int forecast);

public record StateWithElephant(List<string> candidates, ActorState me, ActorState elephant, int timeLeft, int releasing, int released, int loss /*, string history*/);

public record ActorState(string current, int nextMoveTimeLeft);

// 3010 is too low
