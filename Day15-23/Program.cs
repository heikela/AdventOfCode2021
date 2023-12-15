using Common;

var fileName = "../../../input.txt";
// var fileName = "../../../testInput.txt";

string line = File.ReadLines(fileName).First();

int hash(string s)
{
    int result = 0;
    foreach (char c in s)
    {
        result = ((result + c) * 17 % 256);
    }
    return result;
}

Console.WriteLine($"Part 1 : {line.Split(',').Sum(hash)}");

