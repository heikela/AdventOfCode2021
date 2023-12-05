using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

var sections = File.ReadAllLines(fileName).Paragraphs();

List<long> seeds = sections.First().First().Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
List<long> seeds2 = new List<long>();

for (int i = 0; i < seeds.Count - 1; i += 2)
{
    long start = seeds[i];
    for (long offset = 0; offset < seeds[i + 1]; ++offset)
    {
        seeds2.Add(start + offset);
    }
}

List<Mapping> mappings = new List<Mapping>();
foreach (var section in sections.Skip(1))
{
    Mapping mapping = new Mapping();
    mappings.Add(mapping);
    foreach (var line in section.Skip(1))
    {
        mapping.AddRangeDefinition(line);
    }
}

long SeedToLocation(long seed)
{
    return mappings.Aggregate(seed, (n, mapping) => mapping.Transform(n));
}

Console.WriteLine($"Lowest location number = {seeds.Select(SeedToLocation).Min()}");
Console.WriteLine($"Lowest location number with loads of seeds = {seeds2.Select(SeedToLocation).Min()}");

public record Range(long Start, long Len)
{
    public bool Contains(long n)
    {
        return n >= Start && n < Start + Len;
    }
    public long TransformFrom(Range Source, long n)
    {
        if (!Source.Contains(n))
        {
            throw new ArgumentOutOfRangeException("Value to transform not in the given source range");
        }
        return n - Source.Start + Start;
    }
}

public class Mapping
{
    private Dictionary<Range, Range> Ranges = new Dictionary<Range, Range>();
    public Mapping()
    {
    }
    public void AddRangeDefinition(string line)
    {
        List<long> numbers = line.Split(' ').Select(long.Parse).ToList();
        Range source = new Range(numbers[1], numbers[2]);
        Range destination = new Range(numbers[0], numbers[2]);
        Ranges.Add(source, destination);
    }

    public long Transform(long n)
    {
        var applicableMappings = Ranges.Keys.Where(r => r.Contains(n));
        if (applicableMappings.Any())
        {
            Range source = applicableMappings.First();
            return Ranges[source].TransformFrom(source, n);
        }
        else
        {
            return n;
        }
    }
}
