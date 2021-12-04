// See https://aka.ms/new-console-template for more information
using Common;

Func<List<string>, List<List<int>>> parseBoard = lines =>
{
    var rows = lines.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()).ToList();
    var columns = new List<List<int>>();
    for (int pos = 0; pos < rows[0].Count; pos++)
    {
        var column = new List<int>();
        for (int row = 0; row < rows.Count; row++)
        {
            column.Add(rows[row][pos]);
        }
    }
    return rows.Concat(columns).ToList();
};

Func<List<List<int>>, HashSet<int>, Boolean> isWinning = (board, called) => board.Any(rowOrColumn => rowOrColumn.All(number => called.Contains(number)));

Func<List<List<int>>, HashSet<int>, int> uncalled = (board, called) => board.Flatten().ToHashSet().Except(called).ToList().Sum();

var lines = File.ReadLines("input01.txt").ToList();

var callSequence = lines[0].Split(',').Select(int.Parse);

var boardInputs = lines.Skip(1).Paragraphs();

var boards = boardInputs.Select(parseBoard);

var called = new HashSet<int>();

foreach (int call in callSequence)
{
    called.Add(call);
    var winning = boards.Where(b => isWinning(b, called)).ToList();
    foreach(var board in winning)
    {
        Console.WriteLine($"Call {call}, Winning Score: {uncalled(board, called) * call}");
    }
    boards = boards.Where(b => !winning.Contains(b)).ToList();
}
