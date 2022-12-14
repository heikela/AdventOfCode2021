//using Common;

int fullyContains = 0;
int overlaps = 0;
var lines = File.ReadAllLines("../../../input.txt");
foreach (var line in lines)
{
    var elves = line.Split(',');
    int[][] ends = elves.Select(s => s.Split('-').Select(s => int.Parse(s)).ToArray()).ToArray();
    if (ends[0][0] <= ends[1][0] && ends[0][1] >= ends[1][1]) {
        fullyContains++;
    }
    else if (ends[1][0] <= ends[0][0] && ends[1][1] >= ends[0][1])
    {
        fullyContains++;
    }
    if (ends[0][1] >= ends[1][0] && ends[0][0] <= ends[1][1]) {
        overlaps++;
    }
    else if (ends[1][1] >= ends[0][0] && ends[1][0] <= ends[0][1])
    {
        overlaps++;
    }
}

Console.WriteLine(fullyContains);
Console.WriteLine(overlaps);

