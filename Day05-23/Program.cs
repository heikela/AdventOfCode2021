using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

var sections = File.ReadAllLines(fileName).Paragraphs();

List<long> seeds = sections.First().First().Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
List<Range> seeds2 = new List<Range>();

for (int i = 0; i < seeds.Count - 1; i += 2)
{
    long start = seeds[i];
    long len = seeds[i + 1];
    seeds2.Add(new Range(start, len));
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

IEnumerable<Range> SeedsToLocation(Range seeds)
{
    return mappings.Aggregate(new List<Range>() { seeds }, (seedranges, mapping) => seedranges.SelectMany(r => mapping.TransformRange(r)).ToList());
}

Console.WriteLine($"Lowest location number = {seeds.Select(SeedToLocation).Min()}");
Console.WriteLine($"Lowest location number with loads of seeds = {seeds2.SelectMany(SeedsToLocation).Select(r => r.Start).Min()}");

public record Range(long Start, long Len)
{
    public long Beyond()
    {
        return Start + Len;
    }

    public static Range FromStartAndBeyond(long start, long beyond)
    {
        return new Range(start, beyond - start);
    }

    public bool Contains(long n)
    {
        return n >= Start && n < Start + Len;
    }
    public bool Contains(Range r)
    {
        return r.Start >= Start && r.Beyond() <= Beyond();
    }
    public long TransformFrom(Range Source, long n)
    {
        if (!Source.Contains(n))
        {
            throw new ArgumentOutOfRangeException("Value to transform not in the given source range");
        }
        return n - Source.Start + Start;
    }

    public Range TransformFrom(Range Source, Range r)
    {
        if (!Source.Contains(r))
        {
            throw new ArgumentOutOfRangeException("Range to transform not in the given source range");
        }
        return new Range(r.Start - Source.Start + Start, r.Len);
    }

    public bool Intersects(Range other) {
        return other.Start < Beyond() && other.Beyond() > Start;
    }

    public Range Intersection(Range other)
    {
        long start = Math.Max(Start, other.Start);
        long beyond = Math.Min(Beyond(), other.Beyond());
        return new Range(start, beyond - start);
    }

    public IEnumerable<Range> Remainder(Range intersecting)
    {
        Range intersection = Intersection(intersecting);
        if (intersection.Start > Start)
        {
            yield return Range.FromStartAndBeyond(Start, intersection.Start);
        }
        if (intersection.Beyond() < Beyond())
        {
            yield return Range.FromStartAndBeyond(intersection.Beyond(), Beyond());
        }
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

    public IEnumerable<Range> TransformRange(Range range)
    {
        var remainingRanges = new Stack<Range>();
        remainingRanges.Push(range);
        while (remainingRanges.Count > 0)
        {
            var rangeToProcess = remainingRanges.Pop();
            var applicableMappings = Ranges.Keys.Where(r => r.Intersects(rangeToProcess));
            if (applicableMappings.Any())
            {
                Range source = applicableMappings.First();
                Range coveredRange = source.Intersection(rangeToProcess);
                yield return Ranges[source].TransformFrom(source, coveredRange);
                foreach (Range remainder in rangeToProcess.Remainder(coveredRange))
                {
                    remainingRanges.Push(remainder);
                }
            }
            else
            {
                yield return rangeToProcess;
            }
        }
        yield break;
    }
}
