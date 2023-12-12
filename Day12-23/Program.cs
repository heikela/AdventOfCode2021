using Common;
using System.Diagnostics;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();
SpringRecord[] records = lines.Select(SpringRecord.Parse).ToArray();
var bigRecords = records.Select(r => r.FiveFold());

Dictionary<SpringRecord, int> known = new Dictionary<SpringRecord, int>();
Dictionary<IndexPair, long> known2 = new Dictionary<IndexPair, long>();


Console.WriteLine($"Part 1 : {records.Sum(r => r.CountArrangements())}");
long sum = bigRecords.Sum(r => r.CountArrangements());
Console.WriteLine($"Part 2 : {sum}");
Debug.Assert(sum == 157383940585037);

public class SpringRecord
{
    private string IndividualNotes;
    private List<int> BrokenRuns;
    private Dictionary<FastForwardKey, int> NextPossibleRunStart;
    private Dictionary<int, int> LastPossibleStartByRunIndex;
    private Dictionary<IndexPair, long> MemoizedResult;

    private long Memoize(IndexPair x, long result)
    {
        MemoizedResult[x] = result;
        return result;
    }

    private bool IsMemoized(IndexPair x)
    {
        return MemoizedResult.ContainsKey(x);
    }

    private long GetMemoized(IndexPair x)
    {
        return MemoizedResult[x];
    }


    private SpringRecord(string notes, IEnumerable<int> brokenRuns)
    {
        IndividualNotes = notes;
        BrokenRuns = brokenRuns.ToList();

        List<int> relevantLengths = BrokenRuns.ToHashSet().OrderBy(x => x).ToList();
        NextPossibleRunStart = new Dictionary<FastForwardKey, int>();
        for (int pos = 0; pos < IndividualNotes.Length; pos++)
        {
            foreach (int runLength in relevantLengths)
            {
                int candidate = pos;
                FastForwardKey key = new FastForwardKey(pos, runLength);
                IEnumerable<int> possibleStarts = Enumerable.Range(pos, Math.Max(0, IndividualNotes.Length - runLength - pos + 1)).Where(pos => CanAccommodate(runLength, pos));
                if (possibleStarts.Any())
                {
                    NextPossibleRunStart[key] = possibleStarts.First();
                }
                else
                {
                    NextPossibleRunStart[key] = -1;
                }
            }
        }

        LastPossibleStartByRunIndex = new Dictionary<int, int>();
        int end = IndividualNotes.Length;
        for (int runIndex = BrokenRuns.Count - 1; runIndex >= 0; runIndex--)
        {
            int runLength = BrokenRuns[runIndex];
            IEnumerable<int> possibleStarts = Enumerable.Range(0, end - runLength + 1).Reverse().Where(pos => CanAccommodate(runLength, pos));
            int lastPossibleStart = -1;
            if (possibleStarts.Any())
            {
                lastPossibleStart = possibleStarts.First();
            }
            end = lastPossibleStart;

            LastPossibleStartByRunIndex[runIndex] = lastPossibleStart;
        }

        MemoizedResult = new Dictionary<IndexPair, long>();
    }

    public static SpringRecord Parse(string line)
    {
        var parts = line.Split(" ");
        var notes = parts[0];
        var brokenRuns = parts[1].Split(",").Select(int.Parse);
        return new SpringRecord(notes, brokenRuns);
    }

    public SpringRecord FiveFold()
    {
        string repeatedNotes = String.Join("?", Enumerable.Repeat(IndividualNotes, 5));
        List<int> repeatedRuns = Enumerable.Repeat(BrokenRuns, 5).Flatten().ToList();
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

    public long CountArrangements()
    {
        return CountArrangementsRec(0, 0);
    }

    long CountArrangementsRec(int pos, int runIndex)
    {
        IndexPair args = new IndexPair(pos, runIndex);
        if (IsMemoized(args))
        {
            return GetMemoized(args);
        }
        if (runIndex == BrokenRuns.Count)
        {
            if (IndividualNotes.Substring(Math.Min(pos, IndividualNotes.Length)).Any(c => c == '#'))
            {
                return Memoize(args, 0);
            }
            else
            {
                return Memoize(args, 1);
            }
        }
        int runLength = BrokenRuns[runIndex];
        if (!NextPossibleRunStart.ContainsKey(new FastForwardKey(pos, runLength)))
        {
            return Memoize(args, 0);
        }
        int possibleStart = NextPossibleRunStart[new FastForwardKey(pos, runLength)];
        if (possibleStart == -1 || possibleStart > LastPossibleStartByRunIndex[runIndex])
        {
            return Memoize(args, 0);
        }
        else if (IndividualNotes.Substring(pos, possibleStart - pos).Any(c => c == '#'))
        {
            return Memoize(args, 0);
        }
        else if (IndividualNotes[possibleStart] == '#')
        {
            if (CanAccommodate(runLength, possibleStart))
            {
                return Memoize(args, CountArrangementsRec(possibleStart + runLength + 1, runIndex + 1));
            }
            else
            {
                return Memoize(args, 0);
            }
        }
        else if (IndividualNotes[possibleStart] == '?')
        {
            if (CanAccommodate(runLength, possibleStart))
            {
                long countIfStartHere = CountArrangementsRec(possibleStart + runLength + 1, runIndex + 1);
                long countIfNotYet = CountArrangementsRec(possibleStart + 1, runIndex);
                return Memoize(args, countIfStartHere + countIfNotYet);
            }
            else
            {
                return Memoize(args, CountArrangementsRec(possibleStart + 1, runIndex));
            }
        }
        else
        {
            throw new Exception($"Unexpected character {IndividualNotes[possibleStart]} at supposed possible run start position");
        }
    }
}

public record FastForwardKey(int Pos, int RunLength)
{
}

public record IndexPair(int stringPos, int runPos)
{

}
