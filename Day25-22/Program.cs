//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

var result = input.Select(SnafuToInt).Sum();

Console.WriteLine($"Result in decimal is {result}");
Console.WriteLine($"Result in snafu is {IntToSnafu(result)}");

long SnafuToInt(string snafu)
{
    long value = 0;
    long multiplier = 1;
    int pos = snafu.Length - 1;

    while (pos >= 0)
    {
        int digit = -3;
        switch (snafu[pos])
        {
            case '2': digit = 2; break;
            case '1': digit = 1; break;
            case '0': digit = 0; break;
            case '-': digit = -1; break;
            case '=': digit = -2; break;
        }
        if (digit == -3)
        {
            throw new Exception("Didn't parse digit " + snafu[pos]);
        }
        
        value = value + digit * multiplier;

        multiplier = multiplier * 5;
        pos -= 1;
    }
    return value;
}

string IntToSnafu(long n)
{
    Stack<char> digits = new Stack<char>();
    while (n > 0)
    {
        long mod = ((n + 2) % 5) - 2;
        switch (mod)
        {
            case -1: digits.Push('-'); break;
            case -2: digits.Push('='); break;
            case 2: digits.Push('2'); break;
            case 1: digits.Push('1'); break;
            case 0: digits.Push('0'); break;
            default: throw new Exception($"Didn't expect modulus {mod} when converting to Snafu");
        }
        n = n - mod;
        n = n / 5;
    }
    return new string(digits.ToArray());
}


