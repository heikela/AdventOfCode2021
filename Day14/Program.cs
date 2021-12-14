using Common;

var lines = File.ReadAllLines("input14.txt").ToArray();

var polymer = lines[0].Trim();
var start = polymer;

var substitutions = lines.Skip(2).Select(l => { var parts = l.Split(" -> ").ToArray(); return new KeyValuePair<String, String>(parts[0], parts[1]); }).ToDictionary();

for (int step = 0; step < 10; ++step)
{
    polymer = polymer.Aggregate<Char, string>("", (prev, cur) =>
    {
        if (prev == "")
        {
            return prev + cur;
        }
        else
        {
            var pair = prev.Substring(prev.Length - 1) + cur;
            if (substitutions.ContainsKey(pair))
            {
                return prev + substitutions[pair] + cur;
            }
            else
            {
                return prev + cur;
            }
        }
    });
}

var charFrequencies = polymer.GroupBy(c => c).Select(g => new KeyValuePair<Char, long>(g.Key, g.Count())).ToDictionary();
Console.WriteLine($"{charFrequencies.Values.Max() - charFrequencies.Values.Min()}");

Dictionary<String, long> pairsCount = new Dictionary<String, long>();
Dictionary<Char, long> charCount = new Dictionary<Char, long>();

charCount = start.GroupBy(c => c).Select(g => new KeyValuePair<Char, long>(g.Key, g.Count())).ToDictionary();
for (int i = 0; i < start.Length - 1; i++)
{
    string pair = start.Substring(i, 2);
    if (pairsCount.ContainsKey(pair))
    {
        pairsCount[pair]++;
    }
    else
    {
        pairsCount.Add(pair, 1);
    }
}

static void AddToCount<T>(Dictionary<T, long> dict, T val, long count) {
    if (dict.ContainsKey(val))
    {
        dict[val] += count;
    }
    else
    {
        dict.Add(val, count);
    }
}

for (int step = 0; step < 40; ++step)
{
    var newPairsCount = new Dictionary<string, long>();
    foreach (var kv in pairsCount)
    {
        if (substitutions.ContainsKey(kv.Key))
        {
            var subst = substitutions[kv.Key];
            AddToCount(newPairsCount, kv.Key[0] + subst, kv.Value);
            AddToCount(newPairsCount, subst + kv.Key[1], kv.Value);
            AddToCount(charCount, subst[0], kv.Value);
        }
        else
        {
            AddToCount(newPairsCount, kv.Key, kv.Value);
        }
    }
    pairsCount = newPairsCount;
};

Console.WriteLine($"{charCount.Values.Max() - charCount.Values.Min()}");

