//string[] lines = File.ReadAllLines("../../../testInput.txt").ToArray();
string[] lines = File.ReadAllLines("../../../input.txt").ToArray();

static Game AnalyseLine(string line)
{
    var parts = line.Split(':');
    int Id = int.Parse(parts[0].Split(' ')[1]);
    Game result = new Game() { Id = Id };
    foreach (var sample in parts[1].Split(";"))
    {
        foreach (var colourSample in sample.Split(','))
        {
            var trimmedSample = colourSample.Trim();
            int number = int.Parse(trimmedSample.Split(' ')[0]);
            string colour = trimmedSample.Split(' ')[1];
            switch (colour)
            {
                case "red":
                    result.MaxRed = Math.Max(result.MaxRed, number);
                    break;
                case "green":
                    result.MaxGreen = Math.Max(result.MaxGreen, number);
                    break;
                case "blue":
                    result.MaxBlue = Math.Max(result.MaxBlue, number);
                    break;
                default:
                    break;
            }
        }
    }
    return result;
}

static bool PossiblePart1(Game game)
{
    return game.MaxRed <= 12 && game.MaxGreen <= 13 && game.MaxBlue <= 14;
}

static int Power(Game game)
{
    return game.MaxBlue * game.MaxRed * game.MaxGreen;
}

var games = lines.Select(AnalyseLine);

Console.WriteLine(games.Where(PossiblePart1).Select(g => g.Id).Sum());
Console.WriteLine(games.Select(Power).Sum());

public record Game
{
    public int Id;
    public int MaxBlue = 0;
    public int MaxRed = 0;
    public int MaxGreen = 0;
}

// not 5050
