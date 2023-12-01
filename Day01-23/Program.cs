using Common;

//string[] lines = File.ReadAllLines("../../../testInput.txt").ToArray();
string[] lines = File.ReadAllLines("../../../input.txt").ToArray();

Dictionary<string, int> digits = Enumerable.Range(0, 10).Select(n => new KeyValuePair<string, int>(n.ToString(), n)).ToDictionary();

Dictionary<string, int> spelledDigits = new Dictionary<string, int>()
{
    { "one", 1 },
    { "two", 2 },
    { "three", 3 },
    { "four", 4 },
    { "five", 5 },
    { "six", 6 },
    { "seven", 7 },
    { "eight", 8 },
    { "nine", 9 }
};

Dictionary<string, int> allDigits = spelledDigits.Union(digits).ToDictionary();

static Func<string, int> LineToNumber(Dictionary<string, int> digitRepresentations)
{
    return (line) =>
    {
        int firstPos = Int32.MaxValue;
        int lastPos = -1;
        int firstNum = 0;
        int lastNum = 0;

        foreach (string digit in digitRepresentations.Keys)
        {
            int pos = line.IndexOf(digit);
            if (pos != -1 && pos < firstPos)
            {
                firstPos = pos;
                firstNum = digitRepresentations[digit];
            }
            pos = line.LastIndexOf(digit);
            if (pos != -1 && pos > lastPos)
            {
                lastPos = pos;
                lastNum = digitRepresentations[digit];
            }
        }
        return 10 * firstNum + lastNum;
    };
}

int sum = lines.Sum(LineToNumber(digits));
int sum2 = lines.Sum(LineToNumber(allDigits));

Console.WriteLine(sum);
Console.WriteLine(sum2);
