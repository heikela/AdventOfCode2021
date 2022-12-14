using Common;

var lines = File.ReadAllLines("../../../input.txt");
//var lines = File.ReadAllLines("../../../testInput.txt");

int GetPrio(char c)
{
    if (c >= 'a' && c <= 'z')
    {
        return c - 'a' + 1;
    }
    else if (c >= 'A' && c <= 'Z')
    {
        return c - 'A' + 27;
    }
    else
    {
        throw new Exception($"Undefined priority for {c}");
    }
}

char FindDuplicate(string s)
{
    int len = s.Length;

    foreach (char c in s.Substring(0, len / 2))
    {
        if (s.Substring(len / 2).Contains(c))
        {
            return c;
        }
    }
    throw new Exception($"Could not find item that's present in both compartments in {s}");
}

char DetermineBadge(string[] group) {
    if (group.Length != 3)
    {
        throw new Exception($"Expected elves to be in groups of 3 but got a group of {group.Length}");
    }
    Dictionary<char, int> bagCountsByItem = new Dictionary<char, int>();
    foreach (string bag in group)
    {
        HashSet<char> alreadyAccounted = new HashSet<char>();
        foreach (char c in bag)
        {
            if (!alreadyAccounted.Contains(c))
            {
                alreadyAccounted.Add(c);
                bagCountsByItem.AddToCount(c, 1);
            }
        }
    }
    KeyValuePair<char, int> badge = bagCountsByItem.OrderByDescending(kv => kv.Value).First();
    if (badge.Value != 3)
    {
        throw new Exception($"Expected to find a badge in 3 bags but instead found {badge.Key} in {badge.Value} bags in the group starting with {group[0]}");
    }
    return badge.Key;

}

int prioSum = lines.Select(line => GetPrio(FindDuplicate(line))).Sum();

Console.WriteLine(prioSum);

var groups = lines.Chunk(3);

int badgePrioSum = groups.Select(group => GetPrio(DetermineBadge(group))).Sum();

Console.WriteLine(badgePrioSum);


