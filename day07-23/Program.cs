﻿using Common;

//string fileName = "../../../testInput.txt";
string fileName = "../../../input.txt";

string[] lines = File.ReadAllLines(fileName).ToArray();

int cardNumber(char card)
{
    if (card >= '2' && card <= '9')
    {
        return card - '0';
    }
    else
    {
        switch (card)
        {
            case 'T':
                return 10;
            case 'J':
                return 11;
            case 'Q':
                return 12;
            case 'K':
                return 13;
            case 'A':
                return 14;
            default:
                throw new ArgumentOutOfRangeException($"Unknown card {card}");
        }
    }
}

int handType(IEnumerable<char> cards)
{
    var cardGroups = cards.GroupBy(c => c).OrderByDescending(grouping => grouping.Count()).ToList();
    if (cardGroups[0].Count() == 5)
    {
        return 7;
    }
    if (cardGroups[0].Count() == 4)
    {
        return 6;
    }
    if (cardGroups[0].Count() == 3 && cardGroups[1].Count() == 2)
    {
        return 5;
    }
    if (cardGroups[0].Count() == 3)
    {
        return 4;
    }
    if (cardGroups[0].Count() == 2 && cardGroups[1].Count() == 2)
    {
        return 3;
    }
    if (cardGroups[0].Count() == 2)
    {
        return 2;
    }
    return 1;
}

int handPower(string hand)
{
    if (hand.Length != 5)
    {
        throw new ArgumentException($"Hand should have 5 cards but {hand} does not");
    }
    int type = handType(hand);
    int power = type;
    for (int i = 0; i < 5; i++)
    {
        power *= 15;
        power += cardNumber(hand[i]);
    }
    return power;
}

Dictionary<string, int> betsByHand = new Dictionary<string, int>();
foreach (string line in lines)
{
    string hand = line.Split(' ')[0];
    int bet = int.Parse(line.Split(' ')[1]);
    betsByHand[hand] = bet;
}

List<string> handsByRank = betsByHand.Keys.OrderBy(handPower).ToList();

int winnings = 0;
for (int i = 0; i < handsByRank.Count; i++)
{
    int rank = i + 1;
    winnings += rank * betsByHand[handsByRank[i]];
}

Console.WriteLine(winnings);

