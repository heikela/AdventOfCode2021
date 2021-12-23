using System;
using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            State startState = State.FromPositions(new List<string> ()
            {
                "AA1",
                "BA2",
                "CB1",
                "DB2",
                "CC1",
                "AC2",
                "DD1",
                "BD2"
            });
            long cost = DijkstraFrom(startState);
            Console.WriteLine(cost);
        }

        static long DijkstraFrom(State start)
        {
            SimplePriorityQueue<State, int> queue = new SimplePriorityQueue<State, int>();
            Dictionary<State, int> bestSoFar = new Dictionary<State, int>();
            Dictionary<State, State> predecessor = new Dictionary<State, State>();

            queue.Enqueue(start, 0);
            bestSoFar.Add(start, 0);
            predecessor.Add(start, start);
            while (queue.Count != 0)
            {
                State node = queue.Dequeue();
                if (node.IsFinished())
                {
                    return bestSoFar[node];
                }
                foreach ((State node, int edgeWeight) neighbour in node.ValidMoves())
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
            return -1;
        }
    }

    public record State(string normalisedState)
    {
        List<string> Positions()
        {
            return normalisedState.Split(',').Select(arr => new string(arr.ToArray())).ToList();
        }

        public static State FromPositions(List<string> poss)
        {
            return new State(Normalise(poss));
        }

        static string Normalise(List<string> poss)
        {
            return string.Join(",", poss.OrderBy(s => s));
        }

        static string MakePosition(Char area, int posInArea)
        {
            return area.ToString() + posInArea.ToString();
        }

        public bool IsFinished()
        {
            return Positions().All(s => s[0] == s[1]);
        }

        public IEnumerable<(State next, int cost)> ValidMoves()
        {
            List<string> positions = Positions();
            Dictionary<string, Char> occupied = new Dictionary<string, Char>();
            for (int i = 0; i < positions.Count(); ++i)
            {
                Char type = positions[i][0];
                Char area = positions[i][1];
                int posInArea = int.Parse(positions[i].Substring(2));
                occupied.Add(MakePosition(area, posInArea), type);
            }

            for (int i = 0; i < positions.Count(); ++i)
            {
                Char type = positions[i][0];
                Char area = positions[i][1];
                int posInArea = int.Parse(positions[i].Substring(2));
                bool finished = false;
                int costPerStep = 0;
                if (type == area)
                {
                    if (posInArea == 2)
                    {
                        finished = true;
                    }
                    else if (occupied.GetOrElse(MakePosition(area, 2), ' ') == type)
                    {
                        finished = true;
                    }
                }
                if (finished)
                {
                    continue;
                }
                switch (type)
                {
                    case 'A':
                        costPerStep = 1;
                        break;
                    case 'B':
                        costPerStep = 10;
                        break;
                    case 'C':
                        costPerStep = 100;
                        break;
                    case 'D':
                        costPerStep = 1000;
                        break;
                }
                if (area == 'H')
                {
                    bool canMoveIn = true;
                    string dest2 = MakePosition(type, 2);
                    if (occupied.ContainsKey(dest2) && occupied[dest2] != type)
                    {
                        canMoveIn = false;
                    }
                    else if (occupied.ContainsKey(MakePosition(type, 1)))
                    {
                        canMoveIn = false;
                    }
                    int destPos = ((int)(type - 'A')) * 2 + 3;
                    int dir = destPos > posInArea ? 1 : -1;
                    for (int pos = posInArea; pos != destPos; pos += dir)
                    {
                        if (occupied.ContainsKey(MakePosition('H', pos + dir)))
                        {
                            canMoveIn = false;
                        }
                    }
                    if (canMoveIn)
                    {
                        int destPosInRoom = occupied.ContainsKey(dest2) ? 1 : 2;
                        int dist = Math.Abs(posInArea - destPos) + destPosInRoom;
                        int cost = dist * costPerStep;
                        string newEntry = type.ToString() + MakePosition(type, destPosInRoom);
                        List<string> newStateVec = positions.ToList();
                        newStateVec[i] = newEntry;
                        yield return (State.FromPositions(newStateVec), cost);
                    }
                }
                else
                {
                    if (posInArea == 2 && occupied.ContainsKey(MakePosition(area, 1)))
                    {
                        continue;
                    }
                    int currentRoomPos = ((int)(area - 'A')) * 2 + 3;
                    int minPosInHallway = currentRoomPos;
                    int maxPosInHallway = currentRoomPos;
                    if (occupied.ContainsKey(MakePosition('H', currentRoomPos)))
                    {
                        continue;
                    }
                    while (minPosInHallway > 1 && !occupied.ContainsKey(MakePosition('H', minPosInHallway - 1)))
                    {
                        minPosInHallway--;
                    }
                    while (maxPosInHallway < 11 && !occupied.ContainsKey(MakePosition('H', maxPosInHallway + 1)))
                    {
                        maxPosInHallway++;
                    }
                    for (int destPos = minPosInHallway; destPos <= maxPosInHallway; ++destPos)
                    {
                        int dist = Math.Abs(currentRoomPos - destPos) + posInArea;
                        int cost = dist * costPerStep;
                        string newEntry = type.ToString() + MakePosition('H', destPos);
                        List<string> newStateVec = positions.ToList();
                        newStateVec[i] = newEntry;
                        yield return (State.FromPositions(newStateVec), cost);
                    }
                }
            }
            yield break;
        }
    };
}
