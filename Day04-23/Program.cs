using Common;

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

Dictionary<int, long> cardCounts = Enumerable.Range(1, lines.Length).Select(i => new KeyValuePair<int, long>(i, 1)).ToDictionary();

int currentCard = 1;
foreach (var line in lines)
{
    long currentCopies = cardCounts[currentCard];
    int correct = evaluateCard(line);
    for (int offset = 1; offset <= correct; ++offset)
    {
        int cardToCopy = currentCard + offset;
        if (cardCounts.ContainsKey(cardToCopy))
        {
            cardCounts[cardToCopy] += currentCopies;
        }
    }
    ++currentCard;
}

Console.WriteLine($"Part 1: {payout}");
Console.WriteLine($"Part 2: {cardCounts.Values.Sum()}");
