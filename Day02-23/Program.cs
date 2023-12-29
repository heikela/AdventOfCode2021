using Common;

string[] lines = new AoCUtil().GetInput(2023, 2);
//string[] lines = new AoCUtil().GetTestBlock(2023, 2, 0);

static Dictionary<string, int> AnalyseLine(string line)
{
    var parts = line.Split(':');
    int Id = int.Parse(parts[0].Split(' ')[1]);
    Dictionary<string, int> game = new Dictionary<string, int>();
    game["Id"] = Id;
    foreach (var sample in parts[1].Split(";"))
    {
        foreach (var colourSample in sample.Split(','))
        {
            var trimmedSample = colourSample.Trim();
            int number = int.Parse(trimmedSample.Split(' ')[0]);
            string colour = trimmedSample.Split(' ')[1];
            game.AccumulateForKey(Math.Max, 0, colour, number);
        }
    }
    return game;
}

static bool PossiblePart1(Dictionary<string, int> game)
{
    return game["red"] <= 12 && game["green"] <= 13 && game["blue"] <= 14;
}

static int Power(Dictionary<string, int> game)
{
    return game["red"] * game["green"] * game["blue"];
}

var games = lines.Select(AnalyseLine);

Console.WriteLine(games.Where(PossiblePart1).Select(g => g["Id"]).Sum());
Console.WriteLine(games.Select(Power).Sum());
