var input = File.ReadAllLines("../../../input.txt");
//var input = File.ReadAllLines("../../../testInput.txt");

List<Blueprint> blueprints = input.Select(Blueprint.Parse).ToList();

IEnumerable<State> PossibleSuccessors(State current, Blueprint blueprint, int maxTime)
{
    if (current.time == maxTime)
    {
        yield break;
    }
    int maxOreNeed = Math.Max(blueprint.orecost.ore, Math.Max(blueprint.claycost.ore, Math.Max(blueprint.obsidiancost.ore, blueprint.geodecost.ore)));
    int maxClayNeed = blueprint.obsidiancost.clay;
    int maxObsidianNeed = blueprint.geodecost.obsidian;

    // Build no more robots
    yield return new State(current.bots, current.materials + current.bots * (maxTime - current.time), maxTime);
    // Build an ore bot next
    if (current.bots.ore < maxOreNeed)
    {
        int newTime = current.time;
        Stock newMaterials = current.materials;
        while (!newMaterials.GreaterOrEqual(blueprint.orecost))
        {
            newTime++;
            newMaterials = newMaterials + current.bots;
        }
        newTime++;
        newMaterials += current.bots;
        newMaterials -= blueprint.orecost;
        if (newTime <= maxTime)
        {
            yield return new State(current.bots + new Stock(1, 0, 0, 0), newMaterials, newTime);
        }
    }
    // Build a clay bot next
    if (current.bots.clay < maxClayNeed)
    {
        int newTime = current.time;
        Stock newMaterials = current.materials;
        while (!newMaterials.GreaterOrEqual(blueprint.claycost))
        {
            newTime++;
            newMaterials = newMaterials + current.bots;
        }
        newTime++;
        newMaterials += current.bots;
        newMaterials -= blueprint.claycost;
        if (newTime <= maxTime)
        {
            yield return new State(current.bots + new Stock(0, 1, 0, 0), newMaterials, newTime);
        }
    }
    // Build an obsidian bot next
    if (current.bots.clay > 0 && current.bots.obsidian < maxObsidianNeed)
    {
        int newTime = current.time;
        Stock newMaterials = current.materials;
        while (!newMaterials.GreaterOrEqual(blueprint.obsidiancost))
        {
            newTime++;
            newMaterials = newMaterials + current.bots;
        }
        newTime++;
        newMaterials += current.bots;
        newMaterials -= blueprint.obsidiancost;
        if (newTime <= maxTime)
        {
            yield return new State(current.bots + new Stock(0, 0, 1, 0), newMaterials, newTime);
        }
    }
    // Build a geode bot next
    if (current.bots.obsidian > 0)
    {
        int newTime = current.time;
        Stock newMaterials = current.materials;
        while (!newMaterials.GreaterOrEqual(blueprint.geodecost))
        {
            newTime++;
            newMaterials = newMaterials + current.bots;
        }
        newTime++;
        newMaterials += current.bots;
        newMaterials -= blueprint.geodecost;
        if (newTime <= maxTime)
        {
            yield return new State(current.bots + new Stock(0, 0, 0, 1), newMaterials, newTime);
        }
    }
}

int BlueprintQuality(Blueprint blueprint, int maxTime)
{
    return MostGeodes(blueprint, maxTime) * blueprint.number;
}

int MostGeodes(Blueprint blueprint, int maxTime)
{
    int mostGeodes = 0;

    int maxOreNeed = Math.Max(blueprint.orecost.ore, Math.Max(blueprint.claycost.ore, Math.Max(blueprint.obsidiancost.ore, blueprint.geodecost.ore)));
    int maxClayNeed = blueprint.obsidiancost.clay;
    int maxObsidianNeed = blueprint.geodecost.obsidian;

    State start = new State(new Stock(1, 0, 0, 0), new Stock(0, 0, 0, 0), 0);

    Dictionary<State, int> subSolutions = new Dictionary<State, int>();

    int MostGeodesSub(State current)
    {
        if (current.time == maxTime)
        {
            return current.materials.geodes;
        }
        int maxGeodes = 0;
        foreach (State next in PossibleSuccessors(current, blueprint, maxTime))
        {
            int geodes = 0;
            if (subSolutions.ContainsKey(next))
            {
                geodes = subSolutions[next];
            }
            else
            {
                geodes = MostGeodesSub(next);
                subSolutions[next] = geodes;
            }
            if (geodes > maxGeodes)
            {
                maxGeodes = geodes;
            }
        }
        return maxGeodes;
    }

    return MostGeodesSub(start);
}

int totalQualities = blueprints.Select(b => BlueprintQuality(b, 24)).Sum();

Console.WriteLine($"Sum of qualities = {totalQualities}");

Console.WriteLine($"Part 2 = {blueprints.Take(3).Select(b => MostGeodes(b, 32)).Aggregate((a, b) => a * b)}");

record Stock(int ore, int clay, int obsidian, int geodes)
{
    public static Stock operator +(Stock a, Stock b) => new Stock(a.ore + b.ore, a.clay + b.clay, a.obsidian + b.obsidian, a.geodes + b.geodes);
    public static Stock operator -(Stock a, Stock b) => new Stock(a.ore - b.ore, a.clay - b.clay, a.obsidian - b.obsidian, a.geodes - b.geodes);

    public static Stock operator *(int b, Stock a) => new Stock(a.ore * b, a.clay * b, a.obsidian * b, a.geodes * b);
    public static Stock operator *(Stock a, int b) => b * a;

    public bool GreaterOrEqual(Stock other)
    {
        return ore >= other.ore && clay >= other.clay && obsidian >= other.obsidian && geodes >= other.geodes;
    }
}

record Blueprint(int number, Stock orecost, Stock claycost, Stock obsidiancost, Stock geodecost)
{
    public static Blueprint Parse(string line)
    {
        string[] parts = line.Split(' ');
        return new Blueprint(
            // Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.

            int.Parse(parts[1].Substring(0, parts[1].Length - 1)),
            new Stock(int.Parse(parts[6]), 0, 0, 0),
            new Stock(int.Parse(parts[12]), 0, 0, 0),
            new Stock(int.Parse(parts[18]), int.Parse(parts[21]), 0, 0),
            new Stock(int.Parse(parts[27]), 0, int.Parse(parts[30]), 0));
    }
}

record State(Stock bots, Stock materials, int time)
{

}

