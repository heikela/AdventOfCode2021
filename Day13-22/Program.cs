using Common;

var input = File.ReadAllLines("../../../input.txt");
//var input = File.ReadAllLines("../../../testInput.txt");

var pairs = input.Paragraphs();

var result = 0;

int i = 1;
foreach (var pair in pairs)
{
    if (Value.ComparePair(Value.Parse(pair.First()), Value.Parse(pair.Skip(1).First())) == -1)
    {
        result += i;
    }
    ++i;
}

Console.WriteLine(result);

var packets = input.Where(s => s.Trim().Length > 0).Select(Value.Parse).ToList();
packets.Add(new ListValue(new List<Value>() { new ListValue(new List<Value>() { new IntValue(2) }) }));
packets.Add(new ListValue(new List<Value>() { new ListValue(new List<Value>() { new IntValue(6) }) }));

Comparer<Value> comparer = new ValComp();
packets.Sort(comparer);

int prod = 1;
for (i = 0; i < packets.Count; i++)
{
    if (Value.IsDivider(packets[i])) {
        prod *= (i + 1);
    }
}

Console.WriteLine(prod);

abstract class Value
{
    public abstract bool IsIntegerValue();
    public abstract int GetIntegerValue();

    public abstract List<Value> GetValues();

    public static Value Parse(string line)
    {
        var enumerator = line.GetEnumerator();
        enumerator.MoveNext();
        return Parse(enumerator);
    }

    public static bool IsDivider(Value value)
    {
        if (value.IsIntegerValue())
        {
            return false;
        }
        if (value.GetValues().Count != 1)
        {
            return false;
        }
        Value innerList = value.GetValues()[0];
        if (innerList.IsIntegerValue())
        {
            return false;
        }
        if (innerList.GetValues().Count != 1)
        {
            return false;
        }
        Value innerValue = innerList.GetValues()[0];
        if (!innerValue.IsIntegerValue())
        {
            return false;
        }
        if (innerValue.GetIntegerValue() == 2 || innerValue.GetIntegerValue() == 6)
        {
            return true;
        }
        return false;
    }

    public static Value Parse(IEnumerator<char> input)
    {
        bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        if (input.Current == '[')
        {
            List<Value> values = new List<Value>();
            input.MoveNext();
            while (input.Current != ']')
            {
                values.Add(Parse(input));
                if (input.Current == ',')
                {
                    input.MoveNext();
                }
            }
            input.MoveNext();
            return new ListValue(values);
        }
        else
        {
            List<char> digits = new List<char>();
            while (IsDigit(input.Current))
            {
                digits.Add(input.Current);
                input.MoveNext();
            }

            if (digits.Count == 0)
            {
                throw new Exception("Invalid Value");
            }
            return new IntValue(int.Parse(new String(digits.ToArray())));
        }
    }

    public static int ComparePair(Value left, Value right)
    {
        if (left.IsIntegerValue() && right.IsIntegerValue())
        {
            if (left.GetIntegerValue() < right.GetIntegerValue())
            {
                return -1;
            }
            else if (right.GetIntegerValue() < left.GetIntegerValue())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            List<Value> leftList = left.GetValues();
            List<Value> rightList = right.GetValues();

            for (int i = 0; i < leftList.Count && i < rightList.Count; i++)
            {
                int res = ComparePair(leftList[i], rightList[i]);
                if (res != 0)
                {
                    return res;
                }
            }
            return ComparePair(new IntValue(leftList.Count), new IntValue(rightList.Count));
        }
    }
}

class IntValue : Value
{
    private int Val = 0;
    public IntValue(int value)
    {
        Val = value;
    }

    public override bool IsIntegerValue()
    {
        return true;
    }

    public override int GetIntegerValue()
    {
        return Val;
    }

    public override List<Value> GetValues()
    {
        return new List<Value>() { new IntValue(Val) };
    }
}

class ListValue : Value
{
    private List<Value> Values = new List<Value>();
    public ListValue(IEnumerable<Value> values)
    {
        Values = values.ToList();
    }

    public override bool IsIntegerValue()
    {
        return false;
    }

    public override int GetIntegerValue()
    {
        throw new Exception("Illegal operation, trying to use a list as an integer");
    }

    public override List<Value> GetValues()
    {
        return Values;
    }
}

class ValComp : Comparer<Value>
{
    public override int Compare(Value a, Value b)
    {
        return Value.ComparePair(a, b);
    }
}

