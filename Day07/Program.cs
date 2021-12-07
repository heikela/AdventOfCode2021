var crabs = File.ReadAllLines("input07.txt").First().Split(",").Select(int.Parse).ToList();
//var crabs = "16,1,2,0,4,2,7,1,2,14".Split(",").Select(int.Parse).ToList();
var median = crabs.OrderBy(x => x).Skip(crabs.Count() / 2).First();
var median2 = crabs.OrderBy(x => x).Skip(crabs.Count() / 2).First() - 1;

var fuel = crabs.Select(x => Math.Abs(x - median));

Console.WriteLine($"Result = {fuel.Sum()}");

var fuel2 = crabs.Select(x => Math.Abs(x - median2));

Console.WriteLine($"Result = {fuel2.Sum()}");

int mean = (int)Math.Round(crabs.Average());

for (int pos = mean - 2; pos <= mean + 2; pos++)
{
    int fuel3 = crabs.Select(x => (Math.Abs(x - pos) * (Math.Abs(x - pos) + 1) / 2)).Sum();

    Console.WriteLine($"Result = {fuel3}");
}
