using Common;

// See https://aka.ms/new-console-template for more information

var initialCounters = File.ReadLines("input06.txt").First().Split(",").Select(int.Parse).ToList();
//var initialCounters = "3,4,3,1,2".Split(",").Select(int.Parse).ToList();

var fishByAge = initialCounters.GroupBy(x => x).Select(g => new KeyValuePair<int, long>(g.Key, g.Count())).ToDictionary();

const int daysToSimulate = 256;

int day = 0;

while (day < daysToSimulate)
{
    day++;
    var prevAges = fishByAge;
    fishByAge = new Dictionary<int, long>();
    for (int i = 0; i < 8; ++i)
    {
        fishByAge[i] = prevAges.GetOrElse(i + 1, 0);
    }
    fishByAge[6] += prevAges.GetOrElse(0, 0);
    fishByAge[8] = prevAges.GetOrElse(0, 0);
}

Console.WriteLine($"After {day} days there are {fishByAge.Values.Sum()} fish");

