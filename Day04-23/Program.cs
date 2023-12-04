//using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

int evaluateCard(string line)
{
    var parts = line.Split('|');
    var correct = parts[0].Split(':')[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
    var mine = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
    var payout = 0;
    foreach (int n in mine)
    {
        if (correct.Contains(n))
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
    }
    return payout;
}

var payout = lines.Select(evaluateCard).Sum();

Console.WriteLine($"Part 1: {payout}");
Console.WriteLine($"Part 2:");
