using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("../../../../input25.txt");

            var map = lines.Zip(Enumerable.Range(0, 2000), (l, y) => l.AsEnumerable().Zip(Enumerable.Range(0, 2000), (c, x) => new KeyValuePair<Point, char>(new Point(x, y), c))).Flatten().ToDictionary();
            var cucumbers = new Dictionary<Point, char>(map.Where(kv => kv.Value != '.'));

            int maxX = map.Keys.Max(p => p.x);
            int maxY = map.Keys.Max(p => p.y);

            int time = 0;
            bool moved = false;
            do
            {
                moved = false;
                time++;

                HashSet<Point> willMove = new HashSet<Point>(cucumbers
                    .Where(kv => kv.Value == '>')
                    .Where(kv => !cucumbers.ContainsKey(NextPos(kv.Key, kv.Value, maxX, maxY)))
                    .Select(kv => kv.Key));
                if (willMove.Any())
                {
                    moved = true;
                }
                cucumbers = new Dictionary<Point, char>(cucumbers.Select(kv =>
                    {
                        if (willMove.Contains(kv.Key))
                        {
                            return new KeyValuePair<Point, char>(NextPos(kv.Key, kv.Value, maxX, maxY), kv.Value);
                        }
                        else
                        {
                            return kv;
                        }
                    }));
                willMove = new HashSet<Point>(cucumbers
                    .Where(kv => kv.Value == 'v')
                    .Where(kv => !cucumbers.ContainsKey(NextPos(kv.Key, kv.Value, maxX, maxY)))
                    .Select(kv => kv.Key));
                if (willMove.Any())
                {
                    moved = true;
                }
                cucumbers = new Dictionary<Point, char>(cucumbers.Select(kv =>
                {
                    if (willMove.Contains(kv.Key))
                    {
                        return new KeyValuePair<Point, char>(NextPos(kv.Key, kv.Value, maxX, maxY), kv.Value);
                    }
                    else
                    {
                        return kv;
                    }
                }));
            } while (moved);

            Console.WriteLine($"The first time step during which the sea cucumbers didn't move was {time}");
        }

        static Point NextPos(Point pos, char type, int maxX, int maxY)
        {
            if (type == '>')
            {
                Point result = pos + new Point(1, 0);
                if (result.x > maxX)
                {
                    return result with { x = 0 };
                }
                else
                {
                    return result;
                }
            }
            if (type == 'v')
            {
                Point result = pos + new Point(0, 1);
                if (result.y > maxY)
                {
                    return result with { y = 0 };
                }
                else
                {
                    return result;
                }
            }
            throw new Exception($"Unexpected sea cucumber type '{type}'");
        }
    }

    public record Point(int x, int y)
    {
        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    }

}
