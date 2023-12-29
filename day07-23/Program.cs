using Common;

string[] lines = new AoCUtil().GetInput(2023, 7);
//string[] lines = new AoCUtil().GetTestBlock(2023, 7, 0);

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

int cardNumberWithJokers(char card)
{
    if (card == 'J')
    {
        return 1;
    }
    else {
        return cardNumber(card);
    }
}

int handTypeFromCardCounts(List<int> descendingCounts)
{
    if (descendingCounts[0] == 5)
    {
        return 7;
    }
    if (descendingCounts[0] == 4)
    {
        return 6;
    }
    if (descendingCounts[0] == 3 && descendingCounts[1] == 2)
    {
        return 5;
    }
    if (descendingCounts[0] == 3)
    {
        return 4;
    }
    if (descendingCounts[0] == 2 && descendingCounts[1] == 2)
    {
        return 3;
    }
    if (descendingCounts[0] == 2)
    {
        return 2;
    }
    return 1;
}

int handType(IEnumerable<char> cards)
{
    var cardGroups = cards.GroupBy(c => c).Select(grouping => grouping.Count()).OrderByDescending(n => n).ToList();
    return handTypeFromCardCounts(cardGroups);
}

int handTypeWithJokers(IEnumerable<char> cards)
{
    int jokers = cards.Count(c => c == 'J');
    var cardGroups = cards.Where(c => c != 'J').GroupBy(c => c).Select(g => g.Count()).OrderByDescending(n => n).ToList();
    if (jokers == 5)
    {
        cardGroups.Add(5);
    }
    else
    {
        cardGroups[0] += jokers;
    }
    return handTypeFromCardCounts(cardGroups);
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

int handPowerWithJokers(string hand)
{
    if (hand.Length != 5)
    {
        throw new ArgumentException($"Hand should have 5 cards but {hand} does not");
    }
    int type = handTypeWithJokers(hand);
    int power = type;
    for (int i = 0; i < 5; i++)
    {
        power *= 15;
        power += cardNumberWithJokers(hand[i]);
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

List<string> handsByRankWithJokers = betsByHand.Keys.OrderBy(handPowerWithJokers).ToList();
int part2Winnings = 0;
for (int i = 0; i < handsByRankWithJokers.Count; i++)
{
    int rank = i + 1;
    part2Winnings += rank * betsByHand[handsByRankWithJokers[i]];
}

Console.WriteLine(part2Winnings);

