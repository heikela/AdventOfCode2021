using Common;

var lines = File.ReadAllLines("input18.txt").ToList();

/*
var lines =
@"[1,1]
[2,2]
[3,3]
[4,4]
[5,5]
[6,6]".Split("\n");


var lines =
@"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]".Split("\n");

lines =
@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]".Split("\n");
*/


string acc = "";
foreach (var line in lines)
{
    if (acc == "")
    {
        acc = line.Trim();
    }
    else
    {
        acc = add(acc, line.Trim());
//        Console.WriteLine(acc);
    }
}

Console.WriteLine(acc);
Console.WriteLine();
Console.WriteLine(magnitude(parseNumber(acc.GetEnumerator())));

/*
foreach (var line in lines)
{
    parseNumber(line.GetEnumerator());
}
*/

Char parseCharacter(IEnumerator<Char> iterator)
{
    if (!iterator.MoveNext())
    {
        throw new Exception("Unexpected end of input");
    }
    return iterator.Current;
}

void expectCharacter(IEnumerator<Char> iterator, Char expected)
{
    if (!iterator.MoveNext())
    {
        throw new Exception("Unexpected end of input");
    }
    if (iterator.Current != expected)
    {
        throw new Exception($"Expected '{expected}' got '{iterator.Current}'");
    }
}

Number parseNumber(IEnumerator<Char> iterator)
{
    Char first = parseCharacter(iterator);
    if (first == '[')
    {
        Number left = parseNumber(iterator);
        expectCharacter(iterator, ',');
        Number right = parseNumber(iterator);
        expectCharacter(iterator, ']');
        return new Number(left, right, null);
    }
    else
    {
        int normalValue = int.Parse(first.ToString());
        return new Number(null, null, normalValue);
    }
}

int magnitude(Number number)
{
    if (number.normalNumber != null)
    {
        return number.normalNumber.Value;
    }
    else
    {
        return 3 * magnitude(number.left) + 2 * magnitude(number.right);
    }
}

/*
Number add(Number left, Number right)
{
    return reduce(new Number(left, right, null));
}

ExplosionResult explode(Number n, int allowedDepth = 4)
{
    if (n.normalNumber != null)
    {
        return new ExplosionResult(n, false);
    }
    ExplosionResult leftResult = explode(n.left, allowedDepth - 1);
    if (!leftResult.exploded)
    {
        ExplosionResult rightResult = explode(n.right, allowedDepth - 1);
        if (!rightResult.exploded)
        {
            return new ExplosionResult(new Number, )
        }
    }
}
*/

string explode(string number)
{
    int depth = 0;
    Boolean foundExplosion = false;
    Boolean foundPrevNumber = false;
    Boolean foundNextNumber = false;
    int explosionStartPos = -1;
    int prevNumberPos = -1;
    int nextNumberPos = -1;
    int prevNumberLength = -1;
    int nextNumberLength = -1;
    int explosionSize = -1;
    int firstNumberPos = -1;
    int firstNumberLength = -1;
    int secondNumberPos = -1;
    int secondNumberLength = -1;
    for (int i = 0; i < number.Length && !foundExplosion; ++i)
    {
        if (number[i] == '[')
        {
            depth++;
        }
        else if (number[i] == ']')
        {
            depth--;
        }
        if (depth == 5)
        {
            foundExplosion = true;
            explosionStartPos = i;
            firstNumberPos = i + 1;
            int j = firstNumberPos;
            while (number[j] >= '0' && number[j] <= '9')
            {
                j++;
            }
            firstNumberLength = j - firstNumberPos;
            secondNumberPos = j + 1;
            j = secondNumberPos;
            while (number[j] >= '0' && number[j] <= '9')
            {
                j++;
            }
            secondNumberLength = j - secondNumberPos;
            explosionSize = 3 + firstNumberLength + secondNumberLength;
        }
    }
    if (foundExplosion)
    {
        for (int i = explosionStartPos - 1; i >= 0 && !foundPrevNumber; --i)
        {
            if (number[i] >= '0' && number[i] <= '9')
            {
                foundPrevNumber = true;
                prevNumberLength = 1;
                while (number[i - 1] >= '0' && number[i - 1] <= '9')
                {
                    i--;
                    prevNumberLength++;
                }
                prevNumberPos = i;
            }
        }
        for (int i = explosionStartPos + explosionSize; i < number.Length && !foundNextNumber; ++i)
        {
            if (number[i] >= '0' && number[i] <= '9')
            {
                foundNextNumber = true;
                nextNumberLength = 1;
                nextNumberPos = i;
                while (number[i + 1] >= '0' && number[i + 1] <= '9')
                {
                    i++;
                    nextNumberLength++;
                }
            }
        }
        string beforeExplosion;
        if (foundPrevNumber)
        {
            beforeExplosion = number.Substring(0, prevNumberPos) +
                (int.Parse(number.Substring(prevNumberPos, prevNumberLength)) + int.Parse(number.Substring(firstNumberPos, firstNumberLength))).ToString() +
                number.Substring(prevNumberPos + prevNumberLength, explosionStartPos - prevNumberPos - prevNumberLength);
        }
        else
        {
            beforeExplosion = number.Substring(0, explosionStartPos);
        }
        string afterExplosion;
        int afterExplosionPos = explosionStartPos + explosionSize;
        if (foundNextNumber)
        {
            afterExplosion = number.Substring(afterExplosionPos, nextNumberPos - afterExplosionPos) +
                (int.Parse(number.Substring(nextNumberPos, nextNumberLength)) + int.Parse(number.Substring(secondNumberPos, secondNumberLength))).ToString() +
                number.Substring(nextNumberPos + nextNumberLength);
        }
        else
        {
            afterExplosion = number.Substring(afterExplosionPos);
        }
        return beforeExplosion + '0' + afterExplosion;
    }
    else
    {
        return number;
    }
}

string split(string number)
{
    Boolean foundSplit = false;
    int splitStartPos = -1;
    int splitSize = 2;
    Boolean prevIsNumber = false;
    for (int i = 0; i < number.Length && !foundSplit; ++i)
    {
        if (number[i] >= '0' && number[i] <= '9')
        {
            if (prevIsNumber)
            {
                foundSplit = true;
                splitStartPos = i - 1;
            }
            else
            {
                prevIsNumber = true;
            }
        }
        else
        {
            prevIsNumber = false;
        }
    }
    if (foundSplit)
    {
        int numberToSplit = int.Parse(number.Substring(splitStartPos, splitSize));
        int left = numberToSplit / 2;
        int right = (numberToSplit + 1) / 2;
        return number.Substring(0, splitStartPos) + $"[{left},{right}]" + number.Substring(splitStartPos + splitSize);
    }
    else
    {
        return number;
    }
}

string reduce(string number)
{
    string prev = "";
    string current = number;
    do
    {
        prev = current;
        string exploded = explode(prev);
        if (exploded != prev)
        {
            current = exploded;
        }
        else
        {
            current = split(prev);
        }
//        Console.WriteLine(current);
    } while (current != prev);
    return current;
}

string add(string a, string b)
{
    string plainAdd = $"[{a},{b}]";
    Console.WriteLine(plainAdd);
    return reduce(plainAdd);
}

var result = add("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]");
Console.WriteLine($"{result}");

int maxMagnitude = int.MinValue;

for (int i = 0; i < lines.Count; i++)
{
    for (int j = 0; j < lines.Count; ++j)
    {
        if (i == j) continue;
        int m = magnitude(parseNumber(add(lines[i], lines[j]).Trim().GetEnumerator()));
        if (m > maxMagnitude)
        {
            maxMagnitude = m;
        }
    }
}

Console.WriteLine(maxMagnitude);

public record Number(Number left, Number right, int? normalNumber = null);

