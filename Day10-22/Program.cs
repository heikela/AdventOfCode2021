//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");
int cycle = 0;
int x = 1;
int signal = 0;

char[] display = new char[240];
for (int i = 0; i < display.Length; i++)
{
    display[i] = ' ';
}

foreach (string line in input)
{
    int duration = 0;
    int dx = 0;
    if (line.StartsWith("noop"))
    {
        duration = 1;
    } else
    {
        string[] parts = line.Split(' ');
        dx = int.Parse(parts[1]);
        duration = 2;
    }
    bool crossedAThreshold = (cycle + 20 + duration) % 40 != (cycle + 20) % 40 + duration;
    if (crossedAThreshold)
    {
        signal += x * (cycle + 20 + duration - (cycle + 20 + duration) % 40 - 20);
    }
    for (int i = 0; i < duration; i++)
    {
        if (cycle < 241)
        {
            cycle++;
            int relevantPos = (cycle - 1) % 40;
            if (x + 1 >= relevantPos && x - 1 <= relevantPos)
            {
                display[cycle - 1] = '#';
            }
        }
    }
    x += dx;
}
if (cycle < 241)
{
    int relevantPos = (cycle - 1) % 40;
    if (x + 1 >= relevantPos && x - 1 <= relevantPos)
    {
        display[cycle - 1] = '#';
    }
}

Console.WriteLine(signal);

string displayString = new string(display);
for (int line = 0; line < 6; line++)
{
    Console.WriteLine(displayString.Substring(line * 40, 40));
}
