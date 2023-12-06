// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

List<int> durations = new List<int>() { 47, 84, 74, 67 };
List<int> records = new List<int>() { 207, 1394, 1209, 1014 };

bool beatsRecord(long charge, long duration, long record)
{
    return charge * (duration - charge) > record;
}

int result = 1;
for (int i = 0; i < durations.Count; ++i)
{
    int waysToWin = 0;
    for (int charge = 1; charge < durations[i]; ++charge)
    {
        if (beatsRecord(charge,durations[i], records[i]))
        {
            ++waysToWin;
        }
    }
    result *= waysToWin;
}

Console.WriteLine(result);

long duration = 47847467;
long record = 207139412091014;

/* Realise the numbers are small enough you won't need binary search
 * or explicit solution formula

long firstWinUB = duration;
long firstWinLB = 0;
long firstWin = 0;

while (firstWinUB - firstWinLB > 1)
{
    long candidate = (firstWinUB + firstWinLB) / 2;
}*/

long waysToWinPart2 = 0;
for (long charge = 1; charge < duration; ++charge)
{
    if (beatsRecord(charge, duration, record))
    {
        ++waysToWinPart2;
    }
}
Console.WriteLine(waysToWinPart2);

