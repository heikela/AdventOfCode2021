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
            State sampleStart = State.FromPositions(new List<string>()
            {
                "BA1",
                "CA2",
                "CB1",
                "DB2",
                "BC1",
                "CC2",
                "DD1",
                "AD2"
            });
            State startState = State.FromPositions(new List<string>()
            {
                "AA1",
                "BA2",
                "CB1",
                "DB2",
                "CC1",
                "AC2",
                "DD1",
                "BD2",
            });
            State startState2 = State.FromPositions(new List<string>()
            {
                "AA1",
                "BA4",
                "CB1",
                "DB4",
                "CC1",
                "AC4",
                "DD1",
                "BD4",
                "DA2",
                "DA3",
                "CB2",
                "BB3",
                "BC2",
                "AC3",
                "AD2",
                "CD3"
            });
            long cost = AStar(startState2);
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

        static int AStar(State start)
        {
            HashSet<State> visited = new HashSet<State>();
            IPriorityQueue<State, int> frontier = new SimplePriorityQueue<State, int>();

            frontier.Enqueue(start, 0);
            // Do we need to keep track of where we came from, perhaps not...
            Dictionary<State, int> cost = new Dictionary<State, int>();
            cost.Add(start, 0);

            Dictionary<State, int> remainingCost = new Dictionary<State, int>();
            remainingCost.Add(start, start.Heuristic());

            Dictionary<State, int> costEstimate = new Dictionary<State, int>();
            costEstimate.Add(start, cost[start] + remainingCost[start]);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.IsFinished())
                {
                    return costEstimate[current];
                }
                visited.Add(current);
                foreach ((State neighbour, int neighbourCost) n in current.ValidMoves())
                {
                    if (!visited.Contains(n.neighbour))
                    {
                        int tentativeCost = cost[current] + n.neighbourCost;
                        int newEstimate = tentativeCost + n.neighbour.Heuristic();
                        if (!frontier.Contains(n.neighbour))
                        {
                            frontier.Enqueue(n.neighbour, newEstimate);
                            cost.Add(n.neighbour, tentativeCost);
                            costEstimate.Add(n.neighbour, newEstimate);
                        }
                        else if (tentativeCost < cost[n.neighbour])
                        {
                            cost[n.neighbour] = tentativeCost;
                            costEstimate[n.neighbour] = newEstimate;
                            frontier.UpdatePriority(n.neighbour, newEstimate);
                        }
                    }
                }
            }
            return -1;
        }
    }

    public record State(string normalisedState)
    {
        int RoomSize()
        {
            return Positions().Count() / 4;
        }

        List<string> Positions()
        {
            return normalisedState.Split(',').Select(arr => new string(arr.ToArray())).ToList();
        }

        public static int RoomPos(char type)
        {
            return ((int)(type - 'A')) * 2 + 3;
        }

        public static int CostPerStep(char type)
        {
            switch (type)
            {
                case 'A':
                    return 1;
                case 'B':
                    return 10;
                case 'C':
                    return 100;
                case 'D':
                    return 1000;
            }
            throw new Exception("Unexpected type");

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
            int roomSize = RoomSize();
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
                int costPerStep = CostPerStep(type);
                if (type == area)
                {
                    finished = true;
                    for (int furtherPos = posInArea + 1; furtherPos <= roomSize; ++furtherPos)
                    {
                        if (occupied.GetOrElse(MakePosition(type, furtherPos), 'X') != type)
                        {
                            finished = false;
                        }
                    }
                }
                if (finished)
                {
                    continue;
                }
                if (area == 'H')
                {
                    bool canMoveIn = true;
                    int destPosInRoom = roomSize;
                    bool resolved = false;
                    do
                    {
                        var potentialDest = MakePosition(type, destPosInRoom);
                        if (occupied.ContainsKey(potentialDest))
                        {
                            if (occupied[potentialDest] != type)
                            {
                                resolved = true;
                                canMoveIn = false;
                            }
                            else
                            {
                                destPosInRoom--;
                                // There should be no need to stop looping based on index going outside of range due to the design of the problem.
                            }
                        }
                        else
                        {
                            for (int earlierPos = 1; earlierPos < destPosInRoom; ++earlierPos)
                            {
                                if (occupied.ContainsKey(MakePosition(type, earlierPos)))
                                {
                                    resolved = true;
                                    canMoveIn = false;
                                }
                            }
                            if (canMoveIn)
                            {
                                resolved = true;
                            }
                        }
                    } while (!resolved);
                    if (!canMoveIn)
                    {
                        continue;
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
                    bool blocked = false;
                    for (int earlierPos = 1; earlierPos < posInArea; ++earlierPos)
                    {
                        if (occupied.ContainsKey(MakePosition(area, earlierPos)))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked)
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
                        if (destPos == 3 || destPos == 5 || destPos == 7 || destPos == 9)
                        {
                            // no stopping outside of the rooms
                            continue;
                        }
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

        public int Heuristic()
        {
            List<string> positions = Positions();
            int roomSize = RoomSize();
            Dictionary<string, Char> occupied = new Dictionary<string, Char>();
            for (int i = 0; i < positions.Count(); ++i)
            {
                Char type = positions[i][0];
                Char area = positions[i][1];
                int posInArea = int.Parse(positions[i].Substring(2));
                occupied.Add(MakePosition(area, posInArea), type);
            }

            int total = 0;
            for (int i = 0; i < positions.Count(); ++i)
            {
                Char type = positions[i][0];
                Char area = positions[i][1];
                int posInArea = int.Parse(positions[i].Substring(2));
                int costPerStep = CostPerStep(type);
                if (type == area)
                {
                    bool finished = true;
                    for (int laterPos = posInArea + 1; laterPos <= roomSize; ++laterPos)
                    {
                        if (occupied.GetOrElse(MakePosition(type, laterPos), 'X') != type)
                        {
                            finished = false;
                        }
                    }
                    if (finished)
                    {
                        total += (roomSize - posInArea) * costPerStep;
                    }
                    else
                    {
                        total += (posInArea + roomSize + 2) * costPerStep;
                    }
                }
                else if (area == 'H')
                {
                    total += (Math.Abs(RoomPos(type) - posInArea) + roomSize) * costPerStep;
                }
                else
                {
                    total += (Math.Abs(RoomPos(type) - RoomPos(area)) + roomSize + posInArea) * costPerStep;
                }
            }
            return total - (roomSize * (roomSize - 1) / 2) * 1111;
        }
    }

}
