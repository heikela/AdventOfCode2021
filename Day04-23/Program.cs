//using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

int evaluateCard(string line)
{
    var parts = line.Split('|');
    var correct = parts[0].Split(':')[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
    var mine = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
    return mine.Count(n => correct.Contains(n));
}

int part1Points(int matches)
{
    var payout = 0;
    for (int i = 0; i < matches; ++i)
    {
        if (payout == 0)
        {
            payout = 1;
        }
        else
        {
            payout *= 2;
        }
    }
    return payout;
}

var payout = lines.Select(evaluateCard).Select(part1Points).Sum();

Console.WriteLine($"Part 1: {payout}");
Console.WriteLine($"Part 2:");
