// See https://aka.ms/new-console-template for more information

var lines = File.ReadAllLines("input10.txt");
var checkerScore = 0;
var completerScores = new List<long>();

foreach (string line in lines)
{
    Stack<Char> openers = new Stack<Char>();
    foreach (Char c in line)
    {
        Char opener;
        switch (c)
        {
            case '{':
            case '(':
            case '<':
            case '[':
                openers.Push(c);
                break;
            case ')':
                opener = openers.Pop();
                if (opener != '(')
                {
                    checkerScore += 3;
                    goto nextLine;
                }
                break;
            case ']':
                opener = openers.Pop();
                if (opener != '[')
                {
                    checkerScore += 57;
                    goto nextLine;
                }
                break;
            case '}':
                opener = openers.Pop();
                if (opener != '{')
                {
                    checkerScore += 1197;
                    goto nextLine;
                }
                break;
            case '>':
                opener = openers.Pop();
                if (opener != '<')
                {
                    checkerScore += 25137;
                    goto nextLine;
                }
                break;
        }
    }
    long completerScore = 0;
    while (openers.Any())
    {
        Char opener = openers.Pop();
        completerScore *= 5;
        switch (opener)
        {
            case '(':
                completerScore += 1;
                break;
            case '[':
                completerScore += 2;
                break;
            case '{':
                completerScore += 3;
                break;
            case '<':
                completerScore += 4;
                break;
        }
    }
    completerScores.Add(completerScore);
nextLine: //foo
    checkerScore = checkerScore;
}

Console.WriteLine(checkerScore);
Console.WriteLine(completerScores.OrderBy(x => x).Skip(completerScores.Count() / 2).First());
