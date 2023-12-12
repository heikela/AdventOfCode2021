using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();
SpringRecord[] records = lines.Select(SpringRecord.Parse).ToArray();

Console.WriteLine(records.Sum(r => r.CountPossibleArrangements()));

public record SpringRecord(string IndividualNotes, List<int> BrokenRuns)
{
    public static SpringRecord Parse(string line)
    {
        var parts = line.Split(" ");
        var notes = parts[0];
        var brokenRuns = parts[1].Split(",").Select(int.Parse).ToList();
        return new SpringRecord(notes, brokenRuns);
    }

    public int CountPossibleArrangements()
    {
        int charPos = 0;
        int runPos = 0;
        while (charPos < IndividualNotes.Length)
        {

            if (IndividualNotes[charPos] == '.')
            {
                charPos++;
            }
            else if (IndividualNotes[charPos] == '#')
            {
                if (runPos >= BrokenRuns.Count)
                {
//                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                    return 0;
                }
                int runLength = BrokenRuns[runPos];
                runPos++;
                while (runLength > 0)
                {
                    if (charPos >= IndividualNotes.Length || IndividualNotes[charPos] == '.')
                    {
//                        Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                        return 0;
                    }
                    charPos++;
                    runLength--;
                }
                if (charPos == IndividualNotes.Length)
                {
                    if (runPos == BrokenRuns.Count)
                    {
                        //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {1}");
                        return 1;
                    }
                    else
                    {
                        //                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                        return 0;
                    }
                }
                if (IndividualNotes[charPos] != '#')
                {
                    charPos++;
                }
                else
                {
//                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
                    return 0;
                }
            }
            else if (IndividualNotes[charPos] == '?')
            {
/*
                if (IndividualNotes == "?" && BrokenRuns.Count == 1 && BrokenRuns[0] == 1)
                {
                    Console.WriteLine("Error case?");
                }
*/
                bool brokenIsPossible = runPos < BrokenRuns.Count;
                int posIfBroken = charPos;
                int runLength = brokenIsPossible ? BrokenRuns[runPos] : 0;
                while (runLength > 0)
                {
                    if (posIfBroken >= IndividualNotes.Length)
                    {
                        brokenIsPossible = false;
                        break;
                    }
                    if (IndividualNotes[posIfBroken] == '.')
                    {
                        brokenIsPossible = false;
                        break;
                    }
                    posIfBroken++;
                    runLength--;
                }
                if (brokenIsPossible && posIfBroken < IndividualNotes.Length)
                {
                    if (IndividualNotes[posIfBroken] == '#')
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
                    int result = new SpringRecord(IndividualNotes.Substring(charPos + 1), BrokenRuns.Skip(runPos).ToList()).CountPossibleArrangements() +
                        new SpringRecord(IndividualNotes.Substring(posIfBroken), BrokenRuns.Skip(runPos + 1).ToList()).CountPossibleArrangements();
//                    Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {result}");
                    return result;
                }
            }
        }
        if (runPos != BrokenRuns.Count)
        {
//            Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {0}");
            return 0;
        }
//        Console.WriteLine($"{IndividualNotes} {String.Join(',', BrokenRuns.Select(n => n.ToString()))}   ===>   {1}");
        return 1;
    }
}
