using System.Text;

string input = File.ReadAllLines("input16.txt").Single();

string toBinary(string hex)
{
    return new string(hex.SelectMany(c => hexToBinary(c)).ToArray());
}

string binary = toBinary(input);

string hexToBinary(Char c)
{
    switch (c)
    {
        case '0': return "0000";
        case '1': return "0001";
        case '2': return "0010";
        case '3': return "0011";
        case '4': return "0100";
        case '5': return "0101";
        case '6': return "0110";
        case '7': return "0111";
        case '8': return "1000";
        case '9': return "1001";
        case 'A': return "1010";
        case 'B': return "1011";
        case 'C': return "1100";
        case 'D': return "1101";
        case 'E': return "1110";
        case 'F': return "1111";

    };
    return "0000";
}

(int versionSum, int pos) sumVersions(string input, int pos = 0)
{
    int versionSum = 0;
    int version = Convert.ToInt32(input.Substring(pos, 3), 2);
    pos += 3;
    versionSum += version;
    int type = Convert.ToInt32(input.Substring(pos, 3), 2);
    pos += 3;
    if (type == 4)
    {
        Boolean isLast = false;
        do
        {
            isLast = input[pos] == '0';
            pos += 5;
        } while (!isLast);
    }
    else
    {
        Char lengthType = input[pos];
        pos++;
        if (lengthType == '0')
        {
            int subPacketLength = Convert.ToInt32(input.Substring(pos, 15), 2);
            pos += 15;
            int maxPos = pos + subPacketLength;
            while (pos < maxPos)
            {
                (int subPacketSum, pos) = sumVersions(input, pos);
                versionSum += subPacketSum;
            }
        }
        else
        {
            int subPacketCount = Convert.ToInt32(input.Substring(pos, 11), 2);
            pos += 11;
            for (int i = 0; i < subPacketCount; i++)
            {
                (int subPacketSum, pos) = sumVersions(input, pos);
                versionSum += subPacketSum;
            }

        }
    }
    return (versionSum, pos);
}

(long result, int pos) evaluate(string input, int pos = 0)
{
    int version = Convert.ToInt32(input.Substring(pos, 3), 2);
    pos += 3;
    int type = Convert.ToInt32(input.Substring(pos, 3), 2);
    pos += 3;
    if (type == 4)
    {
        Boolean isLast = false;
        long value = 0;
        do
        {
            isLast = input[pos] == '0';
            value = value * 16;
            int hexDigit = Convert.ToInt32(input.Substring(pos + 1, 4), 2);
            value += hexDigit;
            pos += 5;
        } while (!isLast);
        return (value, pos);
    }
    else
    {
        Char lengthType = input[pos];
        pos++;
        if (lengthType == '0')
        {
            int subPacketLength = Convert.ToInt32(input.Substring(pos, 15), 2);
            pos += 15;
            int maxPos = pos + subPacketLength;
            long value = -1;
            long prevSubpacket = -1;
            switch (type)
            {
                case 0:
                    value = 0;
                    break;
                case 1:
                    value = 1;
                    break;
                case 2:
                    value = long.MaxValue;
                    break;
                case 3:
                    value = long.MinValue;
                    break;
                case 5:
                    value = 1;
                    prevSubpacket = long.MaxValue;
                    break;
                case 6:
                    value = 1;
                    prevSubpacket = long.MinValue;
                    break;
                case 7:
                    value = -1;
                    prevSubpacket = -1;
                    break;
            }
            while (pos < maxPos)
            {
                (long subPacketValue, pos) = evaluate(input, pos);
                switch (type)
                {
                    case 0:
                        value += subPacketValue;
                        break;
                    case 1:
                        value *= subPacketValue;
                        break;
                    case 2:
                        value = Math.Min(value, subPacketValue);
                        break;
                    case 3:
                        value = Math.Max(value, subPacketValue);
                        break;
                    case 5:
                        if (!(prevSubpacket > subPacketValue))
                        {
                            value = 0;
                        }
                        prevSubpacket = subPacketValue;
                        break;
                    case 6:
                        if (!(prevSubpacket < subPacketValue))
                        {
                            value = 0;
                        }
                        prevSubpacket = subPacketValue;
                        break;
                    case 7:
                        if (prevSubpacket == -1)
                        {
                            prevSubpacket = subPacketValue;
                            value = 1;
                        }
                        else
                        {
                            if (prevSubpacket != subPacketValue)
                            {
                                value = 0;
                            }
                            prevSubpacket = subPacketValue;
                        }
                        break;
                }
            }
            return (value, pos);
        }
        else
        {
            int subPacketCount = Convert.ToInt32(input.Substring(pos, 11), 2);
            pos += 11;
            long value = -1;
            long prevSubpacket = -1;
            switch (type)
            {
                case 0:
                    value = 0;
                    break;
                case 1:
                    value = 1;
                    break;
                case 2:
                    value = long.MaxValue;
                    break;
                case 3:
                    value = long.MinValue;
                    break;
                case 5:
                    value = 1;
                    prevSubpacket = long.MaxValue;
                    break;
                case 6:
                    value = 1;
                    prevSubpacket = long.MinValue;
                    break;
                case 7:
                    value = -1;
                    prevSubpacket = -1;
                    break;
            }
            for (int i = 0; i < subPacketCount; i++)
            {
                (long subPacketValue, pos) = evaluate(input, pos);
                switch (type)
                {
                    case 0:
                        value += subPacketValue;
                        break;
                    case 1:
                        value *= subPacketValue;
                        break;
                    case 2:
                        value = Math.Min(value, subPacketValue);
                        break;
                    case 3:
                        value = Math.Max(value, subPacketValue);
                        break;
                    case 5:
                        if (!(prevSubpacket > subPacketValue))
                        {
                            value = 0;
                        }
                        prevSubpacket = subPacketValue;
                        break;
                    case 6:
                        if (!(prevSubpacket < subPacketValue))
                        {
                            value = 0;
                        }
                        prevSubpacket = subPacketValue;
                        break;
                    case 7:
                        if (prevSubpacket == -1)
                        {
                            prevSubpacket = subPacketValue;
                            value = 1;
                        }
                        else
                        {
                            if (prevSubpacket != subPacketValue)
                            {
                                value = 0;
                            }
                            prevSubpacket = subPacketValue;
                        }
                        break;
                }
            }
            return (value, pos);
        }
    }
}

Console.WriteLine($"{sumVersions("110100101111111000101000").versionSum}");
Console.WriteLine($"{sumVersions("00111000000000000110111101000101001010010001001000000000").versionSum}");
Console.WriteLine($"{sumVersions("11101110000000001101010000001100100000100011000001100000").versionSum}");
Console.WriteLine($"{sumVersions(binary).versionSum}");

Console.WriteLine($"{evaluate(toBinary("C200B40A82")).result}");
Console.WriteLine($"{evaluate(toBinary("04005AC33890")).result}");
Console.WriteLine($"{evaluate(toBinary("880086C3E88112")).result}");
Console.WriteLine($"{evaluate(toBinary("CE00C43D881120")).result}");
Console.WriteLine($"{evaluate(toBinary("D8005AC2A8F0")).result}");
Console.WriteLine($"{evaluate(toBinary("F600BC2D8F")).result}");
Console.WriteLine($"{evaluate(toBinary("9C005AC2F8F0")).result}");
Console.WriteLine($"{evaluate(toBinary("9C0141080250320F1802104A08")).result}");
Console.WriteLine($"{evaluate(binary).result}");

