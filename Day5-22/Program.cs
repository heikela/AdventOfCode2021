using Common;

var lines = File.ReadAllLines("../../../input.txt");
int stackCount = 9;
//var lines = File.ReadAllLines("../../../testInput.txt");
//int stackCount = 3;

var schematic = lines.Paragraphs().First();
var moves = lines.Paragraphs().Skip(1).First();

List<Stack<char>> stacks = new List<Stack<char>>();
for (int i = 0; i < stackCount; i++)
{
    stacks.Add(new Stack<char>());
}

void Move(int n, int from, int to)
{
    for (int i = 0; i < n; ++i)
    {
        if (stacks[from].Count > 0)
        {
            char c = stacks[from].Pop();
            stacks[to].Push(c);
        }
    }
}

void Move2(int n, int from, int to)
{
    Stack<char> tmp = new Stack<char>();
    for (int i = 0; i < n; ++i)
    {
        if (stacks[from].Count > 0)
        {
            char c = stacks[from].Pop();
            tmp.Push(c);
        }
    }
    while (tmp.Count > 0)
    {
        stacks[to].Push(tmp.Pop());
    }
}

var schematicFromBottom = schematic;
schematicFromBottom.Reverse();
foreach (var line in schematicFromBottom)
{
    if (line.StartsWith(" 1"))
    {
        continue;
    }
    for (int i = 0; i < stackCount; ++i)
    {
        char c = line[i * 4 + 1];
        if (c >= 'A' && c <= 'Z')
        {
            stacks[i].Push(c);
        }
    }
}

foreach (var line in moves)
{
    var parts = line.Split(' ');
    Move(int.Parse(parts[1]), int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1);
}

Console.WriteLine(new String(stacks.Select(s => s.Peek()).ToArray()));

foreach (var stack in stacks)
{
    stack.Clear();
}
foreach (var line in schematicFromBottom)
{
    if (line.StartsWith(" 1"))
    {
        continue;
    }
    for (int i = 0; i < stackCount; ++i)
    {
        char c = line[i * 4 + 1];
        if (c >= 'A' && c <= 'Z')
        {
            stacks[i].Push(c);
        }
    }
}

foreach (var line in moves)
{
    var parts = line.Split(' ');
    Move2(int.Parse(parts[1]), int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1);
}

Console.WriteLine(new String(stacks.Select(s => s.Peek()).ToArray()));
