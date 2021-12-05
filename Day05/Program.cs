// See https://aka.ms/new-console-template for more information
using Common;
using System.Text.RegularExpressions;

var lines = File.ReadLines("input05.txt").ToList();
Regex inputFormat = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)", RegexOptions.Compiled);


Func<string, Line> parseLine = (line) =>
{
    Match m = inputFormat.Match(line);
    if (m.Success)
    {
        return new Line(new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
            new Point(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value)));
    }
    throw new Exception($"Can't parse line {line}");
};

var vents = lines.Select(parseLine);

var simpleVents = vents.Where(l => l.p1.x == l.p2.x || l.p1.y == l.p2.y);
var diagonalVents = vents.Where(l => Math.Abs(l.p1.x - l.p2.x) == Math.Abs(l.p1.y - l.p2.y) && l.p1.x != l.p2.x);

Dictionary<Point, int> ventCount = new Dictionary<Point, int>();

foreach (Line vent in simpleVents)
{
    int x1 = vent.p1.x;
    int x2 = vent.p2.x;
    int y1 = vent.p1.y;
    int y2 = vent.p2.y;

    if (x1 > x2)
    {
        (x2, x1) = (x1, x2);
    }
    if (y1 > y2)
    {
        (y2, y1) = (y1, y2);
    }
    for (int x = x1; x <= x2; x++)
    {
        for (int y = y1; y <= y2; y++)
        {
            Point pos = new Point(x, y);
            if (ventCount.ContainsKey(pos))
            {
                ventCount[pos] += 1;
            }
            else
            {
                ventCount.Add(pos, 1);
            }
        }
    }
}

Console.WriteLine($"Overalapping positions: {ventCount.Values.Count(n => n > 1)}");

foreach (Line vent in diagonalVents)
{
    int x1 = vent.p1.x;
    int x2 = vent.p2.x;
    int y1 = vent.p1.y;
    int y2 = vent.p2.y;

    if (x1 > x2)
    {
        (x2, x1, y2, y1) = (x1, x2, y1, y2);
    }
    int ydir = (y2 > y1) ? 1 : -1;
    int y = y1;
    for (int x = x1; x <= x2; x++, y += ydir)
    {
        Point pos = new Point(x, y);
        if (ventCount.ContainsKey(pos))
        {
            ventCount[pos] += 1;
        }
        else
        {
            ventCount.Add(pos, 1);
        }
    }
}

Console.WriteLine($"Overalapping positions: {ventCount.Values.Count(n => n > 1)}");

public record Point(int x, int y);

public record Line(Point p1, Point p2);

