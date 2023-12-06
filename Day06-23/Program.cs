// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

List<long> durations = new List<long>() { 47, 84, 74, 67 };
List<long> records = new List<long>() { 207, 1394, 1209, 1014 };

bool beatsRecord(long charge, long duration, long record)
{
    return charge * (duration - charge) > record;
}

long countWaysToWin(long duration, long record)
{
    long waysToWin = 0;
    for (long charge = 1; charge < duration; ++charge)
    {
        if (beatsRecord(charge, duration, record))
        {
            ++waysToWin;
        }
    }
    return waysToWin;
}

Console.WriteLine(durations.Zip(records).Select<(long, long), long>(race => countWaysToWin(race.Item1, race.Item2)).Aggregate(1L, (a, b) => a * b));

long duration = 47847467;
long record = 207139412091014;

Console.WriteLine(countWaysToWin(duration, record));
