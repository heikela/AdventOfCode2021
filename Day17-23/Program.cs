using Common;
using Priority_Queue;

string fileName = "../../../input.txt";
//string fileName = "../../../testInput.txt";

string[] lines = File.ReadAllLines(fileName);

Dictionary<Point, int> heatLoss = new Dictionary<Point, int>();

Point end = new Point(0, 0);
for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        Point pos = new Point(x, y);
        heatLoss[new Point(x, y)] = lines[y][x] - '0';
        end = pos;
    }
}

Point left = new Point(-1, 0);
Point right = new Point(1, 0);
Point up = new Point(0, -1);
Point down = new Point(0, 1);

State initialState = new State(new Point(0, 0), down);
State initialState2 = new State(new Point(0, 0), right);

IEnumerable<(State, int)> nextMoves(State state)
{
    Point currentPos = state.Pos;
    Point currentDir = state.Dir;
    Point newPos = currentPos;
    Point leftDir = new Point(currentDir.Y, -currentDir.X);
    Point rightDir = new Point(-currentDir.Y, currentDir.X);
    int cost = 0;
    for (int goStraight = 1; goStraight <= 3; goStraight++)
    {
        newPos += currentDir;
        if (!heatLoss.ContainsKey(newPos))
        {
            break;
        }
        cost += heatLoss[newPos];
        yield return (new State(newPos, leftDir), cost);
        yield return (new State(newPos, rightDir), cost);
    }
}

IEnumerable<(Point, int)> simplifiedBackwardsMoves(Point pos)
{
    if (heatLoss.ContainsKey(pos + left))
    {
        yield return (pos + left, heatLoss[pos]);
    }
    if (heatLoss.ContainsKey(pos + up))
    {
        yield return (pos + up, heatLoss[pos]);
    }
}

Dictionary<Point, int> heuristicDict = new Dictionary<Point, int>();

Action<Point, WeightedGraphByFunction<Point>.VisitPath> recordHeuristic = (node, path) =>
{
    heuristicDict.Add(node, path.GetLength());
};

WeightedGraphByFunction<Point> simplifiedDistance = new WeightedGraphByFunction<Point>(simplifiedBackwardsMoves);
simplifiedDistance.DijkstraFrom(end, recordHeuristic);

IEnumerable<(State, int)> ultraNextMoves(State state)
{
    Point currentPos = state.Pos;
    Point currentDir = state.Dir;
    Point newPos = currentPos;
    Point leftDir = new Point(currentDir.Y, -currentDir.X);
    Point rightDir = new Point(-currentDir.Y, currentDir.X);
    int cost = 0;
    for (int goStraight = 1; goStraight <= 10; goStraight++)
    {
        newPos += currentDir;
        if (!heatLoss.ContainsKey(newPos))
        {
            break;
        }
        cost += heatLoss[newPos];
        if (goStraight < 4)
        {
            continue;
        }
        yield return (new State(newPos, leftDir), cost);
        yield return (new State(newPos, rightDir), cost);
    }
}

int heuristic(State s)
{
    return heuristicDict[s.Pos];
}

WeightedGraphByFunction<State> graph = new WeightedGraphByFunction<State>(nextMoves);

State[] possibleStarts = new State[] { initialState, initialState2 };

int dist = graph.AStarFrom(possibleStarts, heuristic, s => s.Pos == end).GetLength();

Console.WriteLine($"Part 1: {dist}");


WeightedGraphByFunction<State> ultraGraph = new WeightedGraphByFunction<State>(ultraNextMoves);

WeightedGraph<State>.VisitPath path = ultraGraph.AStarFrom(possibleStarts, heuristic, s => s.Pos == end);

Console.WriteLine($"Part 2: {path.GetLength()}");

public record Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
}

public record State(Point Pos, Point Dir);

public abstract class WeightedGraph<T> where T : IEquatable<T>
{
    public class VisitPath
    {
        private int Length;
        private T End;
        private Dictionary<T, T> Predecessors;

        public VisitPath(T end, int length, Dictionary<T, T> predecessors)
        {
            End = end;
            Length = length;
            Predecessors = predecessors;
        }

        public int GetLength()
        {
            return Length;
        }

        public IEnumerable<T> GetNodesOnPath()
        {
            T current = End;
            Stack<T> reversePath = new Stack<T>();
            while (!Predecessors[current].Equals(current))
            {
                T pred = Predecessors[current];
                reversePath.Push(current);
                current = pred;
            }
            while (reversePath.Any())
            {
                yield return reversePath.Pop();
            }
            yield break;
        }
    }

    public abstract IEnumerable<T> GetNodes();

    public abstract IEnumerable<(T, int)> GetNeighbours(T node);

    public VisitPath AStarFrom(IEnumerable<T> starts, Func<T, int> heuristic, Func<T, bool> isGoal)
    {
        SimplePriorityQueue<T, int> queue = new SimplePriorityQueue<T, int>();
        Dictionary<T, int> bestSoFar = new Dictionary<T, int>();
        Dictionary<T, T> predecessor = new Dictionary<T, T>();

        foreach(T start in starts)
        {
            queue.Enqueue(start, heuristic(start));
            bestSoFar.Add(start, 0);
            predecessor.Add(start, start);
        }
        while (queue.Count != 0)
        {
            T node = queue.Dequeue();
            if (isGoal(node))
            {
                return new VisitPath(node, bestSoFar[node], predecessor);
            }
            foreach ((T node, int edgeWeight) neighbour in GetNeighbours(node))
            {
                int potentialDist = bestSoFar[node] + neighbour.edgeWeight;
                int heuristicDist = potentialDist + heuristic(neighbour.node);
                if (potentialDist < bestSoFar.GetOrElse(neighbour.node, int.MaxValue))
                {
                    bestSoFar.AddOrSet(neighbour.node, potentialDist);
                    predecessor.AddOrSet(neighbour.node, node);
                    if (queue.Contains(neighbour.node))
                    {
                        queue.UpdatePriority(neighbour.node, heuristicDist);
                    }
                    else
                    {
                        queue.Enqueue(neighbour.node, heuristicDist);
                    }
                }
            }
        }
        return null;
    }

    public void DijkstraFrom(T start, Func<T, VisitPath, bool> visit)
    {
        SimplePriorityQueue<T, int> queue = new SimplePriorityQueue<T, int>();
        Dictionary<T, int> bestSoFar = new Dictionary<T, int>();
        Dictionary<T, T> predecessor = new Dictionary<T, T>();

        queue.Enqueue(start, 0);
        bestSoFar.Add(start, 0);
        predecessor.Add(start, start);
        while (queue.Count != 0)
        {
            T node = queue.Dequeue();
            if (visit(node, new VisitPath(node, bestSoFar[node], predecessor)))
            {
                return;
            }
            foreach ((T node, int edgeWeight) neighbour in GetNeighbours(node))
            {
                int potentialDist = bestSoFar[node] + neighbour.edgeWeight;
                if (potentialDist < bestSoFar.GetOrElse(neighbour.node, int.MaxValue))
                {
                    bestSoFar.AddOrSet(neighbour.node, potentialDist);
                    predecessor.AddOrSet(neighbour.node, node);
                    if (queue.Contains(neighbour.node))
                    {
                        queue.UpdatePriority(neighbour.node, potentialDist);
                    }
                    else
                    {
                        queue.Enqueue(neighbour.node, potentialDist);
                    }
                }
            }
        }
    }

    public void DijkstraFrom(T start, Action<T, VisitPath> visit)
    {
        Func<T, VisitPath, bool> visitNoShortCircuit = (node, path) =>
        {
            visit(node, path);
            return false;
        };
        DijkstraFrom(start, visitNoShortCircuit);
    }

    public VisitPath ShortestPathTo(T start, T end)
    {
        return ShortestPathTo(start, n => n.Equals(end));
    }

    public VisitPath ShortestPathTo(T start, Func<T, bool> predicate)
    {
        VisitPath pathToGoal = null;
        DijkstraFrom(start, (node, path) =>
        {
            if (predicate(node))
            {
                pathToGoal = path;
                return true;
            }
            else
            {
                return false;
            }
        });
        return pathToGoal;
    }
}

public class ConcreteWeightedGraph<T> : WeightedGraph<T> where T : IEquatable<T>
{
    Dictionary<T, List<(T, int)>> Edges;

    public ConcreteWeightedGraph()
    {
        Edges = new Dictionary<T, List<(T, int)>>();
    }

    public ConcreteWeightedGraph(Dictionary<T, Dictionary<T, int>> edges)
    {
        Edges = new Dictionary<T, List<(T, int)>>();
        foreach (var kv in edges)
        {
            Edges.Add(kv.Key, new List<(T, int)>());
            foreach (var edge in kv.Value)
            {
                Edges[kv.Key].Add((edge.Key, edge.Value));
            }
        }
    }

    public override IEnumerable<T> GetNodes()
    {
        return Edges.Keys;
    }

    public override IEnumerable<(T, int)> GetNeighbours(T node)
    {
        return Edges[node];
    }

    public void AddEdge(T from, T to, int dist)
    {
        if (Edges.ContainsKey(from))
        {
            Edges[from].Add((to, dist));
        }
        else
        {
            Edges.Add(from, new List<(T, int)>() { (to, dist) });
        }
        if (!Edges.ContainsKey(to))
        {
            Edges.Add(to, new List<(T, int)>());
        }
    }
}

public class WeightedGraphByFunction<T> : WeightedGraph<T> where T : IEquatable<T>
{
    private Func<T, IEnumerable<(T, int)>> GetEdges;

    public WeightedGraphByFunction(Func<T, IEnumerable<(T, int)>> getEdges)
    {
        GetEdges = getEdges;
    }

    public override IEnumerable<(T, int)> GetNeighbours(T node)
    {
        return GetEdges(node);
    }

    public override IEnumerable<T> GetNodes()
    {
        throw new Exception("Not implemented");
    }
}

