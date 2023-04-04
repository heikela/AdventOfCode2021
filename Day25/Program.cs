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


            var listing = File.ReadAllLines("../../../../day07-22-input.txt");
            Directory root = new Directory();
            Stack<Directory> wd = new Stack<Directory>();
            wd.Push(root);
            foreach (var line in listing)
            {
                if (line.StartsWith("$ ls")) {
                }
                else if (line.StartsWith("$ cd /"))
                {
                    wd.Clear();
                    wd.Push(root);
                }
                else if (line.StartsWith("$ cd .."))
                {
                    wd.Pop();
                }
                else if (line.StartsWith("$ cd"))
                {
                    string dirName = line.Split(' ')[2];
                    if (!wd.Peek().Directories.ContainsKey(dirName))
                    {
                        wd.Peek().Directories.Add(dirName, new Directory());
                    }
                    wd.Push(wd.Peek().Directories[dirName]);
                }
                else if (line.StartsWith("dir"))
                {

                }
                else
                {
                    var parts = line.Split(' ');
                    int size = int.Parse(parts[0]);
                    string fileName = parts[1];
                    if (!wd.Peek().Files.ContainsKey(fileName))
                    {
                        wd.Peek().Files.Add(fileName, size);
                    }
                }
            }

            int totalOfUnder100k = 0;

            ProcessPart1(root, ref totalOfUnder100k);

            Console.WriteLine($"The sum of sizes of smallish directories is {totalOfUnder100k}");

            int spaceNeeded = Size(root) - 40000000;

            int bestSize = int.MaxValue;

            ProcessPart2(root, spaceNeeded, ref bestSize);

            Console.WriteLine($"The size of the best directory to delete is {bestSize}");
        }

        static int ProcessPart1(Directory dir, ref int totalOfSmall)
        {
            int size = 0;
            foreach (Directory subDir in dir.Directories.Values)
            {
                size += ProcessPart1(subDir, ref totalOfSmall);
            }
            size += dir.Files.Values.Sum();
            if (size <= 100000)
            {
                totalOfSmall += size;
            }
            return size;
        }

        static int ProcessPart2(Directory dir, int spaceNeeded, ref int bestSize)
        {
            int size = 0;
            foreach (Directory subDir in dir.Directories.Values)
            {
                size += ProcessPart2(subDir, spaceNeeded, ref bestSize);
            }
            size += dir.Files.Values.Sum();
            if (size >= spaceNeeded && size < bestSize)
            {
                bestSize = size;
            }
            return size;
        }

        static int Size(Directory dir)
        {
            int size = 0;
            foreach (Directory subDir in dir.Directories.Values)
            {
                size += Size(subDir);
            }
            size += dir.Files.Values.Sum();
            return size;
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

    public class Directory
    {
        public Dictionary<string, Directory> Directories = new Dictionary<string, Directory>();
        public Dictionary<string, int> Files = new Dictionary<string, int>();
    }

}
