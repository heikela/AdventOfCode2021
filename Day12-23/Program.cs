using Common;
using System.Diagnostics;

string fileName = "../../../testInput.txt";
//string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();
SpringRecord[] records = lines.Select(SpringRecord.Parse).ToArray();
var bigRecords = records.Select(r => r.FiveFold());

Dictionary<SpringRecord, int> known = new Dictionary<SpringRecord, int>();
Dictionary<IndexPair, long> known2 = new Dictionary<IndexPair, long>();

bool CanFit(SpringRecord record)
{
    int startPos = 0;
    int runPos = 0;
    while (runPos < record.BrokenRuns.Length)
    {
        int runLength = record.BrokenRuns[runPos];
        bool found = false;
        while (!found)
        {
            while (startPos < record.IndividualNotes.Length && record.IndividualNotes[startPos] == '.')
            {
                startPos++;
            }
            if (startPos >= record.IndividualNotes.Length)
            {
                return false;
            }
            found = true;
            for (int i = 0; i < runLength; i++)
            {
                if (startPos + i >= record.IndividualNotes.Length)
                {
                    return false;
                }
                if (record.IndividualNotes[startPos + i] == '.')
                {
                    found = false;
                    startPos = startPos + i + 1;
                    break;
                }
            }
        }
        runPos++;
    }
    return true;
}


int CountPossibleArrangements(SpringRecord record)
{
    if (IsMemoized(record))
    {
        return GetMemoized(record);
    }
    int minLenght = record.BrokenRuns.Sum() + record.BrokenRuns.Length - 1;
    if (record.IndividualNotes.Length < minLenght)
    {
        //          Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
        Memoize(record, 0);
        return 0;
    }
    if (!CanFit(record))
    {
        //            Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
        Memoize(record, 0);
        return 0;
    }
    int charPos = 0;
    int runPos = 0;
    while (charPos < record.IndividualNotes.Length)
    {

        if (record.IndividualNotes[charPos] == '.')
        {
            charPos++;
        }
        else if (record.IndividualNotes[charPos] == '#')
        {
            if (runPos >= record.BrokenRuns.Length)
            {
                //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                Memoize(record, 0);
                return 0;
            }
            int runLength = record.BrokenRuns[runPos];
            runPos++;
            while (runLength > 0)
            {
                if (charPos >= record.IndividualNotes.Length || record.IndividualNotes[charPos] == '.')
                {
                    //                        Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                    Memoize(record, 0);
                    return 0;
                }
                charPos++;
                runLength--;
            }
            if (charPos == record.IndividualNotes.Length)
            {
                if (runPos == record.BrokenRuns.Length)
                {
                    //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {1}");
                    Memoize(record, 1);
                    return 1;
                }
                else
                {
                    //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                    Memoize(record, 0);
                    return 0;
                }
            }
            if (record.IndividualNotes[charPos] != '#')
            {
                charPos++;
            }
            else
            {
                //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                Memoize(record, 0);
                return 0;
            }
        }
        else if (record.IndividualNotes[charPos] == '?')
        {
            /*
                            if (IndividualNotes == "?" && BrokenRuns.Count == 1 && BrokenRuns[0] == 1)
                            {
                                Console.WriteLine("Error case?");
                            }
            */
            bool brokenIsPossible = runPos < record.BrokenRuns.Length;
            int posIfBroken = charPos;
            int runLength = brokenIsPossible ? record.BrokenRuns[runPos] : 0;
            while (runLength > 0)
            {
                if (posIfBroken >= record.IndividualNotes.Length)
                {
                    brokenIsPossible = false;
                    break;
                }
                if (record.IndividualNotes[posIfBroken] == '.')
                {
                    brokenIsPossible = false;
                    break;
                }
                posIfBroken++;
                runLength--;
            }
            if (brokenIsPossible && posIfBroken < record.IndividualNotes.Length)
            {
                if (record.IndividualNotes[posIfBroken] == '#')
                {
                    brokenIsPossible = false;
                }
                posIfBroken++;
            }
            if (!brokenIsPossible)
            {
                charPos++;
            }
            else
            {
                int result = CountPossibleArrangements(new SpringRecord(record.IndividualNotes.Substring(charPos + 1), record.BrokenRuns.Skip(runPos).ToArray())) +
                    CountPossibleArrangements(new SpringRecord(record.IndividualNotes.Substring(posIfBroken), record.BrokenRuns.Skip(runPos + 1).ToArray()));
                //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {result}");
                Memoize(record, result);
                return result;
            }
        }
    }
    if (runPos != record.BrokenRuns.Length)
    {
        //            Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
        Memoize(record, 0);
        return 0;
    }
    //        Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {1}");
    Memoize(record, 1);
    return 1;
}

long sum1 = 0;
foreach (var record in records)
{
    long result = CountArrangements2(record);
    sum1 += result;
    Console.WriteLine($"{record}   ===>   {result}");
}
Console.WriteLine($"Part 1 : {sum1}");

void Memoize(SpringRecord record, int result)
{

    //known[record] = result;
}

bool IsMemoized(SpringRecord record)
{
    return false; // known.ContainsKey(record);
}

int GetMemoized(SpringRecord record)
{
    return known[record];
}

long Memoize2(IndexPair x, long result)
{
    known2[x] = result;
    return result;
}

bool IsMemoized2(IndexPair x)
{
    return known2.ContainsKey(x);
}

long GetMemoized2(IndexPair x)
{
    return known2[x];
}

void ResetMemo2()
{
    known2 = new Dictionary<IndexPair, long>();
}

long CountArrangementsRec(SpringRecord record, int pos, int runIndex, Dictionary<FastForwardKey, int> nextPossibleRunStart, Dictionary<int, int> lastPossibleStartByRunIndex)
{
    IndexPair args = new IndexPair(pos, runIndex);
    if (IsMemoized2(args))
    {
        return GetMemoized2(args);
    }
    if (runIndex == record.BrokenRuns.Length)
    {
        if (record.IndividualNotes.Substring(Math.Min(pos, record.IndividualNotes.Length)).Any(c => c == '#'))
        {
            return Memoize2(args, 0);
        }
        else
        {
            return Memoize2(args, 1);
        }
    }
    int runLength = record.BrokenRuns[runIndex];
    if (!nextPossibleRunStart.ContainsKey(new FastForwardKey(pos, runLength)))
    {
        return Memoize2(args, 0);
    }
    int possibleStart = nextPossibleRunStart[new FastForwardKey(pos, runLength)];
    if (possibleStart == -1 || possibleStart > lastPossibleStartByRunIndex[runIndex])
    {
        return Memoize2(args, 0);
    }
    else if (record.IndividualNotes.Substring(pos, possibleStart - pos).Any(c => c == '#'))
    {
        return Memoize2(args, 0);
    }
    else if (record.IndividualNotes[possibleStart] == '#')
    {
        if (record.CanAccommodate(runLength, possibleStart))
        {
            if (runIndex == record.BrokenRuns.Length - 1)
            {
                return Memoize2(args, 1);
            }
            else
            {
                return Memoize2(args, CountArrangementsRec(record, possibleStart + runLength + 1, runIndex + 1, nextPossibleRunStart, lastPossibleStartByRunIndex));
            }
        }
        else
        {
            return Memoize2(args, 0);
        }
    }
    else if (record.IndividualNotes[possibleStart] == '?')
    {
        if (record.CanAccommodate(runLength, possibleStart))
        {
            long countIfStartHere = CountArrangementsRec(record, possibleStart + runLength + 1, runIndex + 1, nextPossibleRunStart, lastPossibleStartByRunIndex);
            long countIfNotYet = CountArrangementsRec(record, possibleStart + 1, runIndex, nextPossibleRunStart, lastPossibleStartByRunIndex);
            return Memoize2(args, countIfStartHere + countIfNotYet);
        }
        else
        {
            return Memoize2(args, CountArrangementsRec(record, possibleStart + 1, runIndex, nextPossibleRunStart, lastPossibleStartByRunIndex));
        }
    }
    else
    {
        throw new Exception($"Unexpected character {record.IndividualNotes[possibleStart]} at supposed possible run start position");
    }
}

long CountArrangements2(SpringRecord record)
{
    List<int> relevantLengths = record.BrokenRuns.ToHashSet().OrderBy(x => x).ToList();
    Dictionary<FastForwardKey, int> nextPossibleRunStart = new Dictionary<FastForwardKey, int>();
    for (int pos = 0; pos < record.IndividualNotes.Length; pos++)
    {
        foreach (int runLength in relevantLengths)
        {
            int candidate = pos;
            FastForwardKey key = new FastForwardKey(pos, runLength);
            while (candidate + runLength <= record.IndividualNotes.Length)
            {
                if (record.IndividualNotes.Substring(candidate, runLength).All(c => c != '.'))
                {
                    nextPossibleRunStart[key] = candidate;
                    break;
                }
                else
                {
                    candidate++;
                }
            }
            if (!nextPossibleRunStart.ContainsKey(key))
            {
                nextPossibleRunStart[key] = -1;
            }
        }
    }

    Dictionary<int, int> lastPossibleStartByRunIndex = new Dictionary<int, int>();
    int end = record.IndividualNotes.Length;
    for (int runIndex = record.BrokenRuns.Length - 1; runIndex >= 0; runIndex--)
    {
        int runLength = record.BrokenRuns[runIndex];
        IEnumerable<int> possibleStarts = Enumerable.Range(0, end - runLength + 1).Reverse().Where(pos => record.CanAccommodate(runLength, pos));
        int lastPossibleStart = -1;
        if (possibleStarts.Any())
        {
            lastPossibleStart = possibleStarts.First();
        }
        end = lastPossibleStart;

        lastPossibleStartByRunIndex[runIndex] = lastPossibleStart;
    }

    ResetMemo2();
    return CountArrangementsRec(record, 0, 0, nextPossibleRunStart, lastPossibleStartByRunIndex);
}

//Debug.Assert(CountArrangements2(new SpringRecord("", new int[] {})) == 1);
Debug.Assert(CountArrangements2(new SpringRecord("", new int[] { 1 })) == 0);
Debug.Assert(CountArrangements2(new SpringRecord(".", new int[] { 1 })) == 0);
//Debug.Assert(CountArrangements2(new SpringRecord(".", new int[] {})) == 1);
Debug.Assert(CountArrangements2(new SpringRecord("#", new int[] { 1 })) == 1);
//Debug.Assert(CountArrangements2(new SpringRecord("#", new int[] {})) == 0);
Debug.Assert(CountArrangements2(new SpringRecord("??", new int[] {1})) == 2);

long sum = 0;
foreach (var record in bigRecords)
{
    long result = CountArrangements2(record);
    sum += result;
    Console.WriteLine(result);
}

Console.WriteLine("Part 2 result " + sum);
// 157385579854051 is too high

public record SpringRecord(string IndividualNotes, int[] BrokenRuns)
{
    public static SpringRecord Parse(string line)
    {
        var parts = line.Split(" ");
        var notes = parts[0];
        var brokenRuns = parts[1].Split(",").Select(int.Parse).ToArray();
        return new SpringRecord(notes, brokenRuns);
    }

    public SpringRecord FiveFold()
    {
        string repeatedNotes = String.Join("?", Enumerable.Repeat(IndividualNotes, 5));
        int[] repeatedRuns = Enumerable.Repeat(BrokenRuns, 5).Flatten().ToArray();
        return new SpringRecord(repeatedNotes, repeatedRuns);
    }

    public bool CanAccommodate(int runLength, int pos)
    {
        bool posOK = pos + runLength <= IndividualNotes.Length && pos >= 0;
        bool contentsOK = IndividualNotes.Substring(pos, runLength).All(c => c != '.');
        bool beforeOK = pos == 0 || IndividualNotes[pos - 1] != '#';
        int afterPos = pos + runLength;
        bool afterOK = afterPos >= IndividualNotes.Length || IndividualNotes[afterPos] != '#';
        return posOK && contentsOK && beforeOK && afterOK;
    }

    public override string ToString()
    {
        return $"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}";
    }

}

public record FastForwardKey(int Pos, int RunLength)
{
}

public record IndexPair(int stringPos, int runPos)
{

}
