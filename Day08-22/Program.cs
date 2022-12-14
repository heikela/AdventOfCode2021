// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

int[][] trees = File.ReadAllLines("../../../input.txt").Select(line => line.Select(c => c - '0').ToArray()).ToArray();

bool[][] visible = trees.Select(t => t.Select(n => false).ToArray()).ToArray();

int h = trees.Length;
int w = trees[0].Length;

int dx = 0;
int dy = 1;
int reqHeight = 0;
for (int startX = 0, startY = 0; startX < w; startX++)
{
    reqHeight = 0;
    for (int x = startX, y = startY; x < w && y < h && x >= 0 && y >= 0; x += dx, y += dy)
    {
        int height = trees[y][x];
        if (height >= reqHeight)
        {
            visible[y][x] = true;
            reqHeight = height + 1;
        }
        if (reqHeight > 9)
        {
            break;
        }
    }
}

dx = 1;
dy = 0;
reqHeight = 0;
for (int startX = 0, startY = 0; startY < h; startY++)
{
    reqHeight = 0;
    for (int x = startX, y = startY; x < w && y < h && x >= 0 && y >= 0; x += dx, y += dy)
    {
        int height = trees[y][x];
        if (height >= reqHeight)
        {
            visible[y][x] = true;
            reqHeight = height + 1;
        }
        if (reqHeight > 9)
        {
            break;
        }
    }
}

dx = -1;
dy = 0;
reqHeight = 0;
for (int startX = w - 1, startY = 0; startY < h; startY++)
{
    reqHeight = 0;
    for (int x = startX, y = startY; x < w && y < h && x >= 0 && y >= 0; x += dx, y += dy)
    {
        int height = trees[y][x];
        if (height >= reqHeight)
        {
            visible[y][x] = true;
            reqHeight = height + 1;
        }
        if (reqHeight > 9)
        {
            break;
        }
    }
}

dx = 0;
dy = -1;
reqHeight = 0;
for (int startX = 0, startY = h - 1; startX < h; startX++)
{
    reqHeight = 0;
    for (int x = startX, y = startY; x < w && y < h && x >= 0 && y >= 0; x += dx, y += dy)
    {
        int height = trees[y][x];
        if (height >= reqHeight)
        {
            visible[y][x] = true;
            reqHeight = height + 1;
        }
        if (reqHeight > 9)
        {
            break;
        }
    }
}

Console.WriteLine(visible.Select(line => line.Count(v => v)).Sum());

/*
foreach (var line in trees)
{
    foreach (int height in line)
    {
        Console.Write(height);
    }
    Console.WriteLine();
}
Console.WriteLine();

foreach (var line in visible)
{
    foreach (bool v in line)
    {
        Console.Write(v ? 'X' : '.');
    }
    Console.WriteLine();
}
Console.WriteLine();
*/

int ScenicScore(int x, int y, int[][] trees)
{
    return CountInDirection(x, y, -1, 0, trees) *
        CountInDirection(x, y, 1, 0, trees) *
        CountInDirection(x, y, 0, -1, trees) *
        CountInDirection(x, y, 0, 1, trees);
}

int CountInDirection(int startX, int startY, int dx, int dy, int[][] trees)
{
    int tooHigh = trees[startY][startX];
    int visibleCount = 0;
    for (int x = startX + dx, y = startY + dy; x < w && y < h && x >= 0 && y >= 0; x += dx, y += dy)
    {
        visibleCount++;
        if (trees[y][x] >= tooHigh)
        {
            break;
        }
    }
    return visibleCount;
}

int bestScore = 0;
for (int y = 0; y < h; ++y)
{
    for (int x = 0; x < w; ++x)
    {
        int score = ScenicScore(x, y, trees);
        if (score > bestScore)
        {
            bestScore = score;
        }
    }
}

Console.WriteLine(bestScore);
