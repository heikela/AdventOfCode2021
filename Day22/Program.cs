using Common;

var lines = File.ReadLines("input22.txt");
//var lines = File.ReadLines("sampleInput22b.txt");

Instruction parseLine(string line)
{
    var parts1 = line.Split(' ').ToArray();
    bool on = parts1[0] == "on";
    var coordinates = parts1[1].Split(',').Select(s => s.Substring(2).Split("..").Select(int.Parse).ToArray()).ToArray();
    return new Instruction(on, new Rectangle(new Point(coordinates[0][0], coordinates[1][0], coordinates[2][0]),
        new Point(coordinates[0][1], coordinates[1][1], coordinates[2][1])));
}

bool Part1Instruction(Instruction instruction)
{
    return instruction.area.Intersect(new Rectangle(new Point(-50, -50, -50), new Point(50, 50, 50))).Size() > 0;
}

var instructions = lines.Select(parseLine);

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

HashSet<Point> part1State = new HashSet<Point>();

foreach (var instruction in instructions)
{
    if (Part1Instruction(instruction))
    {
        for (int x = instruction.area.min.x; x <= instruction.area.max.x; ++x)
        {
            for (int y = instruction.area.min.y; y <= instruction.area.max.y; ++y)
            {
                for (int z = instruction.area.min.z; z <= instruction.area.max.z; ++z)
                {
                    Point pos = new Point(x, y, z);
                    if (instruction.on)
                    {
                        part1State.Add(pos);
                    }
                    else
                    {
                        part1State.Remove(pos);
                    }
                }
            }
        }
    }
}

Console.WriteLine(part1State.Count());

public record Point(int x, int y, int z);

public record Rectangle(Point min, Point max)
{
    public Rectangle Intersect(Rectangle other)
    {
        return new Rectangle(new Point(Math.Max(this.min.x, other.min.x), Math.Max(this.min.y, other.min.y), Math.Max(this.min.z, other.min.z)),
            new Point(Math.Min(this.max.x, other.max.x), Math.Min(this.max.y, other.max.y), Math.Min(this.max.z, other.max.z)));
    }

    public long Size()
    {
        return Math.Max(0, max.x - min.x + 1) * Math.Max(0, max.y - min.y + 1) * Math.Max(0, max.z - min.z + 1);
    }
}



public record Instruction(bool on, Rectangle area);


