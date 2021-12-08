using Common;

var lines = File.ReadAllLines("input08.txt");

List<List<string>> patterns = new List<List<string>>();
List<List<string>> outputs = new List<List<string>>();

foreach (var line in lines)
{
    var parts = line.Split("|");
    patterns.Add(parts[0].Trim().Split(" ").ToList());
    outputs.Add(parts[1].Trim().Split(" ").ToList());
}

Console.WriteLine(outputs.Flatten().Count(x => x.Length == 2 || x.Length == 3 || x.Length == 4 || x.Length == 7));

Dictionary<Char, string> digits = new Dictionary<Char, string>();
digits.Add('0', "abcefg");
digits.Add('1', "cf");
digits.Add('2', "acdeg");
digits.Add('3', "acdfg");
digits.Add('4', "bcdf");
digits.Add('5', "abdfg");
digits.Add('6', "abdefg");
digits.Add('7', "acf");
digits.Add('8', "abcdefg");
digits.Add('9', "abcdfg");

Dictionary<string, Char> usefulDigits = digits.Select(kv => new KeyValuePair<string, char>(kv.Value, kv.Key)).ToDictionary();

Func<List<string>, Dictionary<Char, Char>> Analyze = (input) =>
{
    Dictionary<Char, HashSet<Char>> possibleSegments = new Dictionary<Char, HashSet<Char>>();
    Dictionary<Char, Char> decodedSegments = new Dictionary<Char, Char>();
    Dictionary<Char, Char> encodedSegments = new Dictionary<Char, Char>();
    Dictionary<Char, HashSet<Char>> knownNumbers = new Dictionary<Char, HashSet<Char>>();
    for (Char c = 'a'; c <= 'g'; ++c)
    {
        var p = new HashSet<Char>();
        for (Char c2 = 'a'; c2 <= 'g'; ++c2)
        {
            p.Add(c2);
        }
        possibleSegments[c] = p;
    }
    /* TODO general algorightm 
    foreach (string sample in input)
    {
        var possibleNumbers = digits.Select(kv => kv.Value)
    }
    */
    knownNumbers.Add('1', input.First(x => x.Length == 2).ToHashSet());
    knownNumbers.Add('7', input.First(x => x.Length == 3).ToHashSet());
    knownNumbers.Add('4', input.First(x => x.Length == 4).ToHashSet());
    knownNumbers.Add('8', input.First(x => x.Length == 7).ToHashSet());

    Char aEncoded = knownNumbers['7'].Except(knownNumbers['1']).First();
    decodedSegments.Add(aEncoded, 'a');
    encodedSegments.Add('a', aEncoded);
    knownNumbers.Add('2', input.Where(x => x.Length == 5).Select(x => x.ToHashSet()).First(x => x.Intersect(knownNumbers['4']).Count() == 2));
    knownNumbers.Add('5', input.Where(x => x.Length == 5).Select(x => x.ToHashSet()).First(x => x.Except(knownNumbers['2']).Count() == 2));
    Char fEncoded = knownNumbers['1'].Intersect(knownNumbers['5']).First();
    decodedSegments.Add(fEncoded, 'f');
    encodedSegments.Add('f', fEncoded);
    Char gEncoded = knownNumbers['5'].Except(knownNumbers['4']).Except(knownNumbers['7']).First();
    decodedSegments.Add(gEncoded, 'g');
    encodedSegments.Add('g', gEncoded);
    Char bEncoded = knownNumbers['5'].Except(knownNumbers['1']).Except(knownNumbers['2']).First();
    decodedSegments.Add(bEncoded, 'b');
    encodedSegments.Add('b', bEncoded);
    Char cEncoded = knownNumbers['1'].Except(knownNumbers['5']).First();
    decodedSegments.Add(cEncoded, 'c');
    encodedSegments.Add('c', cEncoded);
    Char dEncoded = knownNumbers['4'].Except(knownNumbers['1']).First(c => c != bEncoded);
    decodedSegments.Add(dEncoded, 'd');
    encodedSegments.Add('d', dEncoded);
    Char eEncoded = "abcdefg".Except(decodedSegments.Keys).First();
    decodedSegments.Add(eEncoded, 'e');
    encodedSegments.Add('e', eEncoded);
    return decodedSegments;
};

int sum = 0;
for (int i = 0; i < outputs.Count(); ++i)
{
    var decoding = Analyze(patterns[i]);
    var decodedDisplay = new string(outputs[i].Select(s => usefulDigits[new string(s.Select(c => decoding[c]).OrderBy(c => c).ToArray())]).ToArray());
    var value = int.Parse(decodedDisplay);
    sum += value;
}

Console.WriteLine(sum);

/*
a THE one in 3-long not in 2-long.
b (one of the two in 4-long not in 2-long), THE one present in the 5-long that shares 3 with the 4-long and is not shared with the 2-long nor the other 5-long
c one of the two in 2-long .. THE one not shared with the 5-long that shares 3 with the 4-long
d one of the two in 4-long not in 2-long, THE one that's not B
e THE remaining one
f one of the two in 2-long .. THE one shared with the 5-long that shares 3 with the 4-long
g THE one present in the 5-long that shares 3 with the 4-long and is not a nor shared with 4-long
*/

