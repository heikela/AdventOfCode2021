//var input = File.ReadAllLines("../../../testInput.txt");
var input = File.ReadAllLines("../../../input.txt");

List<LinkedListNode<long>> orderOfMoves = new List<LinkedListNode<long>>();
LinkedList<long> file = new LinkedList<long>();

foreach (var line in input)
{
    long num = long.Parse(line.Trim());
    file.AddLast(num);
    orderOfMoves.Add(file.Last);
}

Mix(file, orderOfMoves);
long coordinates = GetCoordinates(file);

Console.WriteLine($"coords = {coordinates}");

file.Clear();
orderOfMoves.Clear();
foreach (var line in input)
{
    long num = long.Parse(line.Trim()) * 811589153;
    file.AddLast(num);
    orderOfMoves.Add(file.Last);
}

for (int i = 0; i < 10; ++i)
{
    Mix(file, orderOfMoves);
}

coordinates = GetCoordinates(file);

Console.WriteLine($"coords = {coordinates}");

void Mix(LinkedList<long> file, List<LinkedListNode<long>> orderOfMoves)
{
    foreach (LinkedListNode<long> node in orderOfMoves)
    {
        long num = node.Value;
        long count = num;
        count = count % (orderOfMoves.Count - 1);
        while (count != 0)
        {
            if (count > 0)
            {
                count--;
                if (node.Next == null)
                {
                    file.Remove(node);
                    file.AddFirst(node);
                }
                LinkedListNode<long> next = node.Next;
                file.Remove(next);
                file.AddBefore(node, next);
            }
            if (count < 0)
            {
                count++;
                if (node.Previous == null)
                {
                    file.Remove(node);
                    file.AddLast(node);
                }
                LinkedListNode<long> prev = node.Previous;
                file.Remove(prev);
                file.AddAfter(node, prev);
            }
        }
//        Console.WriteLine(string.Join(", ", file));
    }
}

long GetCoordinates(LinkedList<long> file)
{
    LinkedListNode<long> node = file.Find(0);
    if (node == null)
    {
        throw new Exception($"This shouldn't happen, can't find 0 in file");
    }
    long result = 0;
    for (long i = 1; i <= 3000; ++i)
    {
        if (node == null)
        {
            throw new Exception($"This shouldn't happen, we've lost the node in iterating through the list");
        }
        if (node.Next != null)
        {
            node = node.Next;
        }
        else
        {
            node = file.First;
        }
        if (i % 1000 == 0)
        {
            Console.WriteLine($"Adding up {node.Value}");
            result += node.Value;
        }
    }
    return result;
}
