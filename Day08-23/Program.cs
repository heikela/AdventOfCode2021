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

(int, string) countSteps(string from, Func<string, bool> endCondition, int instructionShift = 0)
{
    string pos = from;

    int step = 0;
    do
    {
        char instruction = instructions[(step + instructionShift) % instructions.Length];
        if (instruction == 'L')
        {
            pos = left[pos];
        }
        else
        {
            pos = right[pos];
        }
        step++;
    } while (!endCondition(pos));
    return (step, pos);
}

Console.WriteLine(countSteps("AAA", s => s == "ZZZ").Item1);

Dictionary<(string, int), (string, int)> nextZ = new Dictionary<(string, int), (string, int)>();
Dictionary<(string, int), int> stepsToNextZ = new Dictionary<(string, int), int>();
var initials = left.Keys.Where(pos => pos.EndsWith('A'));
foreach (var start in initials)
{
    (int steps, string pos) = countSteps(start, s => s.EndsWith('Z'));
    Console.WriteLine($"Discovered path with {steps} steps from {start} to {pos} with instruction shift {0}");
    nextZ.Add((start, 0), (pos, steps));
    stepsToNextZ.Add((start, 0), steps);
}

bool added = false;
do
{
    added = false;
    foreach (var start in nextZ.Values.ToList())
    {
        if (!nextZ.ContainsKey(start))
        {
            (int steps, string pos) = countSteps(start.Item1, s => s.EndsWith('Z'), start.Item2);
            Console.WriteLine($"Discovered path with {steps} steps from {start} to {pos} with instruction shift {start.Item2}");
            nextZ.Add(start, (pos, (start.Item2 + steps) % instructions.Length));
            stepsToNextZ.Add(start, steps);
            added = true;
        }
    }

} while (added);

// Surprising results printed, which means to solve, all that is needed was to do least common multiple (e.g. with Wolfram)

/*
long step = 0;
while (!positions.All(s => s.EndsWith('Z')))
{
    char instruction = instructions[(int)(step % instructions.Length)];
    if (instruction == 'L')
    {
        positions = positions.Select(s => left[s]).ToList();
    }
    else
    {
        positions = positions.Select(s => right[s]).ToList();
    }
    step++;
}

Console.WriteLine(step);
*/

