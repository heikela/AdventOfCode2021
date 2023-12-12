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

public abstract class DynamicProgrammingProblem<TResult, TSubproblemState>
{
    private Dictionary<TSubproblemState, TResult> MemoizedResult;

    protected DynamicProgrammingProblem()
    {
        MemoizedResult = new Dictionary<TSubproblemState, TResult>();
    }
    private TResult Memoize(TSubproblemState x, TResult result)
    {
        MemoizedResult[x] = result;
        return result;
    }

    private bool IsMemoized(TSubproblemState x)
    {
        return MemoizedResult.ContainsKey(x);
    }

    private TResult GetMemoized(TSubproblemState x)
    {
        return MemoizedResult[x];
    }

    protected abstract TResult SolveSubproblem(TSubproblemState subProblemState);

    protected TResult SolveAndMemoizeSubproblem(TSubproblemState subProblemState)
    {
        if (IsMemoized(subProblemState))
        {
            return GetMemoized(subProblemState);
        }
        else
        {
            TResult result = SolveSubproblem(subProblemState);
            Memoize(subProblemState, result);
            return result;
        }
    }

}

public class SpringRecord : DynamicProgrammingProblem<long, IndexPair>
{
    private string IndividualNotes;
    private List<int> BrokenRuns;
    private Dictionary<FastForwardKey, int> NextPossibleRunStart;
    private Dictionary<int, int> LastPossibleStartByRunIndex;

    private SpringRecord(string notes, IEnumerable<int> brokenRuns) : base()
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
        return SolveSubproblem(new IndexPair(0, 0));
    }

    protected override long SolveSubproblem(IndexPair subProblemState)
    {
        int pos = subProblemState.stringPos;
        int runIndex = subProblemState.runPos;
        if (runIndex == BrokenRuns.Count)
        {
            if (IndividualNotes.Substring(Math.Min(pos, IndividualNotes.Length)).Any(c => c == '#'))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        int runLength = BrokenRuns[runIndex];
        if (!NextPossibleRunStart.ContainsKey(new FastForwardKey(pos, runLength)))
        {
            return 0;
        }
        int possibleStart = NextPossibleRunStart[new FastForwardKey(pos, runLength)];
        if (possibleStart == -1 || possibleStart > LastPossibleStartByRunIndex[runIndex])
        {
            return 0;
        }
        else if (IndividualNotes.Substring(pos, possibleStart - pos).Any(c => c == '#'))
        {
            return 0;
        }
        else if (IndividualNotes[possibleStart] == '#')
        {
            if (CanAccommodate(runLength, possibleStart))
            {
                return SolveAndMemoizeSubproblem(new IndexPair(possibleStart + runLength + 1, runIndex + 1));
            }
            else
            {
                return 0;
            }
        }
        else if (IndividualNotes[possibleStart] == '?')
        {
            if (CanAccommodate(runLength, possibleStart))
            {
                long countIfStartHere = SolveAndMemoizeSubproblem(new IndexPair(possibleStart + runLength + 1, runIndex + 1));
                long countIfNotYet = SolveAndMemoizeSubproblem(new IndexPair(possibleStart + 1, runIndex));
                return countIfStartHere + countIfNotYet;
            }
            else
            {
                return SolveAndMemoizeSubproblem(new IndexPair(possibleStart + 1, runIndex));
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
