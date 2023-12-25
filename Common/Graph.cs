using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public abstract class Graph<T>
    {
        public class VisitPath
        {
            private int Length;
            private T Start;
            private T End;
            private Dictionary<T, T> Predecessors;

            public VisitPath(T end, T start, int length, Dictionary<T, T> predecessors)
            {
                End = end;
                Length = length;
                Start = start;
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
                while (!current.Equals(Start))
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

        public abstract IEnumerable<T> GetNeighbours(T node);

        public void BfsFrom(T start, Func<T, VisitPath, bool> visit)
        {
            Dictionary<T, T> visited = new Dictionary<T, T>();
            Dictionary<T, T> frontier = new Dictionary<T, T>();
            frontier.Add(start, start);
            int depth = 0;
            bool earlyExitRequested = false;
            while (frontier.Any() && !earlyExitRequested)
            {
                Dictionary<T, T> newFrontier = new Dictionary<T, T>();
                foreach ((T node, T predecessor) in frontier)
                {
                    if (visited.ContainsKey(node))
                    {
                        // How does this happen?
                        continue;
                    }
                    visited.Add(node, predecessor);
                    VisitPath path = new VisitPath(node, start, depth, visited);
                    earlyExitRequested = visit(node, path) || earlyExitRequested;
                    foreach (T neighbour in GetNeighbours(node))
                    {
                        if (!visited.ContainsKey(neighbour) && !newFrontier.ContainsKey(neighbour))
                        {
                            newFrontier.Add(neighbour, node);
                        }
                    }
                }
                frontier = newFrontier;
                depth += 1;
            }
        }

        public void BfsFrom(T start, Action<T, VisitPath> visit)
        {
            Func<T, VisitPath, bool> visitNoShortCircuit = (node, path) =>
            {
                visit(node, path);
                return false;
            };
            BfsFrom(start, visitNoShortCircuit);
        }

        public VisitPath ShortestPathTo(T start, T end)
        {
            return ShortestPathTo(start, n => n.Equals(end));
        }

        public VisitPath ShortestPathTo(T start, Func<T, bool> predicate)
        {
            VisitPath pathToGoal = null;
            BfsFrom(start, (node, path) =>
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

        private void TopologicalSortVisit(HashSet<T> fullyVisited, HashSet<T> partiallyVisited, Stack<T> sorted, T node)
        {
            if (fullyVisited.Contains(node))
            {
                return;
            }
            if (partiallyVisited.Contains(node))
            {
                throw new Exception("Not a DAG");
            }
            partiallyVisited.Add(node);
            foreach (T neighbour in GetNeighbours(node))
            {
                TopologicalSortVisit(fullyVisited, partiallyVisited, sorted, neighbour);
            }
            partiallyVisited.Remove(node);
            fullyVisited.Add(node);
            sorted.Push(node);
        }

        public IEnumerable<T> TopologicalSort()
        {
            HashSet<T> fullyVisited = new HashSet<T>();
            HashSet<T> partiallyVisited = new HashSet<T>();
            Stack<T> sorted = new Stack<T>();
            foreach (T node in GetNodes())
            {
                TopologicalSortVisit(fullyVisited, partiallyVisited, sorted, node);
            }
            while (sorted.Any())
            {
                yield return sorted.Pop();
            }
            yield break;
        }

    }

    public class ConcreteGraph<T> : Graph<T>
    {
        Dictionary<T, List<T>> Edges;

        public ConcreteGraph()
        {
            Edges = new Dictionary<T, List<T>>();
        }

        public ConcreteGraph(Dictionary<T, List<T>> edges)
        {
            Edges = edges;
        }

        public override IEnumerable<T> GetNodes()
        {
            return Edges.Keys;
        }

        public override IEnumerable<T> GetNeighbours(T node)
        {
            return Edges[node];
        }

        public void AddEdge(T from, T to)
        {
            if (Edges.ContainsKey(from))
            {
                Edges[from].Add(to);
            }
            else
            {
                Edges.Add(from, new List<T>() { to });
            }
            if (!Edges.ContainsKey(to))
            {
                Edges.Add(to, new List<T>());
            }
        }

        public ConcreteGraph<T> ReverseGraph()
        {
            ConcreteGraph<T> reverse = new ConcreteGraph<T>();
            foreach (T source in GetNodes())
            {
                foreach (T dest in GetNeighbours(source))
                {
                    reverse.AddEdge(dest, source);
                }
            }
            return reverse;
        }

        public IEnumerable<List<T>> KargerMinimumCut(int goodenough = 3)
        {
            int minCut = int.MaxValue;
            Dictionary<T, T> originalNodesInMinCut = null;

            Random random = new Random();

            int nodeCount = GetNodes().Count();
            int ln = (int)Math.Log(nodeCount);
            int iterations = nodeCount * nodeCount * ln;

            int iteration = 0;
            while (minCut > goodenough && iteration < iterations)
            {
                iteration += 1;
                ConcreteGraph<T> contracted = new ConcreteGraph<T>(Edges.Select(kv => new KeyValuePair<T, List<T>>(kv.Key, kv.Value.ToList())).ToDictionary());
                Dictionary<T, T> originalNodes = new Dictionary<T, T>();
                while (contracted.GetNodes().Count() > 2)
                {
                    T[] nodes = contracted.GetNodes().ToArray();
                    T node1 = nodes[random.Next(nodes.Length)];
                    T[] neighbours = contracted.GetNeighbours(node1).ToArray();
                    T node2 = neighbours[random.Next(neighbours.Length)];
                    originalNodes.Add(node2, node1);
                    contracted.ContractEdge(node1, node2);
                }
                int cut = contracted.GetNeighbours(contracted.GetNodes().First()).Count();
                if (cut < minCut)
                {
                    minCut = cut;
                    originalNodesInMinCut = originalNodes.ToDictionary();
                }
            }
            Dictionary<T, List<T>> halves = new Dictionary<T, List<T>>();
            foreach (T node in GetNodes())
            {
                T current = node;
                while (originalNodesInMinCut.ContainsKey(current))
                {
                    current = originalNodesInMinCut[current];
                }
                if (halves.ContainsKey(current))
                {
                    halves[current].Add(node);
                }
                else
                {
                    halves.Add(current, new List<T>() { node });
                }
            }
            return halves.Values;
        }

        private void ContractEdge(T node1, T node2)
        {
            List<T> newNeighbours = new List<T>();
            foreach (T neighbour in GetNeighbours(node1))
            {
                if (!neighbour.Equals(node2))
                {
                    newNeighbours.Add(neighbour);
                }
            }
            foreach (T neighbour in GetNeighbours(node2))
            {
                if (!neighbour.Equals(node1))
                {
                    newNeighbours.Add(neighbour);
                }
            }
            Edges[node1] = newNeighbours;
            Edges.Remove(node2);
            foreach (T neighbour in Edges[node1].ToHashSet())
            {
                List<T> neighbours = Edges[neighbour];
                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (neighbours[i].Equals(node2))
                    {
                        neighbours[i] = node1;
                    }
                }
            }
        }

    }

    public class GraphByFunction<T> : Graph<T> where T : IEquatable<T>
    {
        private Func<T, IEnumerable<T>> GetEdges;

        public GraphByFunction(Func<T, IEnumerable<T>> getEdges)
        {
            GetEdges = getEdges;
        }

        public override IEnumerable<T> GetNeighbours(T node)
        {
            return GetEdges(node);
        }

        public override IEnumerable<T> GetNodes()
        {
            throw new Exception("Not implemented");
        }
    }
}