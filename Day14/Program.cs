using Common;

var lines = File.ReadAllLines("input14.txt").ToArray();

var polymer = lines[0].Trim();

var substitutions = lines.Skip(2).Select(l => { var parts = l.Split(" -> ").ToArray(); return new KeyValuePair<String, String>(parts[0], parts[1]); }).ToDictionary();

for (int step = 0; step < 40; ++step)
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
