// See https://aka.ms/new-console-template for more information

//string[] lines = File.ReadAllLines("../../../testInput.txt").ToArray();
string[] lines = File.ReadAllLines("../../../input.txt").ToArray();

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

int sum = 0;
int sum2 = 0;

foreach (var line in lines)
{
    char firstDigit = '0';
    try
    {
        firstDigit = line.First(c => c >= '0' && c <= '9');
    }
    catch (InvalidOperationException e)
    {
    }
    char lastDigit = '0';
    try
    {
        lastDigit = line.Reverse().First(c => c >= '0' && c <= '9');
    }
    catch (InvalidOperationException e)
    {
    }
        
    int firstNum = firstDigit - '0';
    int lastNum = lastDigit - '0';
    int num = 10 * firstNum + lastNum;
    sum += num;

    int firstPos = line.IndexOf(firstDigit);
    if (firstPos == -1) {
        firstPos = Int32.MaxValue;
    }
    int lastPos = line.LastIndexOf(lastDigit);

    foreach(string spelledDigit in spelledDigits.Keys)
    {
        int pos = line.IndexOf(spelledDigit);
        if (pos != -1 && pos < firstPos) {
            firstPos = pos;
            firstNum = spelledDigits[spelledDigit];
        }
        pos = line.LastIndexOf(spelledDigit);
        if (pos != -1 && pos > lastPos)
        {
            lastPos = pos;
            lastNum = spelledDigits[spelledDigit];
        }
    }
    num = 10 * firstNum + lastNum;
    sum2 += num;
}
Console.WriteLine(sum);
Console.WriteLine(sum2);


