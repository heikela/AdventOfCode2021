using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

var sections = File.ReadAllLines(fileName).Paragraphs().ToList();

string instructions = sections[0].First();

Dictionary<string, string> left = new Dictionary<string, string>();
Dictionary<string, string> right = new Dictionary<string, string>();

foreach (var line in sections[1])
{
    left.Add(line.Substring(0, 3), line.Substring(7, 3));
    right.Add(line.Substring(0, 3), line.Substring(12, 3));
}

string pos = "AAA";
string end = "ZZZ";

int step = 0;
while (pos != end)
{
    char instruction = instructions[step % instructions.Length];
    if (instruction == 'L')
    {
        pos = left[pos];
    }
    else
    {
        pos = right[pos];
    }
    step++;
}

Console.WriteLine(step);
