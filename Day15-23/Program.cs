var fileName = "../../../input.txt";
//var fileName = "../../../testInput.txt";

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

List<List<KeyValuePair<string, int>>> boxes = Enumerable.Range(0, 256).Select(i => new List<KeyValuePair<string, int>>()).ToList();

foreach (string step in line.Split(','))
{
    char[] operators = { '=', '-' };
    string[] parts = step.Split(operators);
    string name = parts[0];
    int lens = 0;
    char op = step[name.Length];
    if ( op == '=')
    {
        lens = int.Parse(step.Substring(name.Length + 1));
    }
    int box = hash(name);
    if (op == '-')
    {
        boxes[box].RemoveAll(kv => kv.Key == name);
    }
    if (op == '=')
    {
        int existingIndex = boxes[box].FindIndex(kv => kv.Key == name);
        if (existingIndex >= 0)
        {
            boxes[box][existingIndex] = new KeyValuePair<string, int>(name, lens);
        }
        else
        {
            boxes[box].Add(new KeyValuePair<string, int>(name, lens));
        }
    }
}

int power(int boxIndex, List<KeyValuePair<string, int>> contents)
{
    int result = 0;
    for (int i = 0; i < contents.Count; ++i)
    {
        result += (boxIndex + 1) * (i + 1) * contents[i].Value;
    }
    return result;
}

int part2 = 0;
for (int i = 0; i < 256; ++i)
{
    part2 += power(i, boxes[i]);
}

Console.WriteLine($"Part 2 : {part2}");

