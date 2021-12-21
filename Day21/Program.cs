using Common;

int rolls = 0;
int dieVal = 0;

int Roll()
{
    rolls++;
    if (dieVal >= 100)
    {
        dieVal = 0;
    }
    dieVal++;
    return dieVal;
}

int s1 = 0;
int s2 = 0;

int p1 = 4;
int p2 = 2;

while (s1 < 1000 && s2 < 1000)
{
    p1 += Roll();
    p1 += Roll();
    p1 += Roll();
    while (p1 > 10)
    {
        p1 -= 10;
    }
    s1 += p1;
    if (s1 >= 1000)
    {
        break;
    }
    p2 += Roll();
    p2 += Roll();
    p2 += Roll();
    while (p2 > 10)
    {
        p2 -= 10;
    }
    s2 += p2;
}

Console.WriteLine($"{Math.Min(s1, s2) * rolls}");



Dictionary<State, long> stateCount = new Dictionary<State, long>();

stateCount.Add(new State(4, 2, 0, 0, true, false), 1);

for (int scoreSum = 0; scoreSum < 42; scoreSum++)
{
    foreach (var kv in stateCount.Where(kv => kv.Key.s1 + kv.Key.s2 == scoreSum && kv.Key.p1Active && !kv.Key.won).ToList())
    {
        State prev = kv.Key;
        stateCount.Remove(prev);
        long prevCount = kv.Value;
        for (int r1 = 1; r1 <= 3; r1++)
        {
            for (int r2 = 1; r2 <= 3; r2++)
            {
                for (int r3 = 1; r3 <= 3; r3++)
                {
                    int newPos = prev.p1 + r1 + r2 + r3;
                    while (newPos > 10)
                    {
                        newPos -= 10;
                    }
                    int newScore = prev.s1 + newPos;
                    State newState = new State(newPos, prev.p2, newScore, prev.s2, false, newScore >= 21);
                    stateCount.AddToCount(newState, prevCount);
                }
            }
        }
    }
    foreach (var kv in stateCount.Where(kv => kv.Key.s1 + kv.Key.s2 == scoreSum && !kv.Key.p1Active && !kv.Key.won).ToList())
    {
        State prev = kv.Key;
        stateCount.Remove(prev);
        long prevCount = kv.Value;
        for (int r1 = 1; r1 <= 3; r1++)
        {
            for (int r2 = 1; r2 <= 3; r2++)
            {
                for (int r3 = 1; r3 <= 3; r3++)
                {
                    int newPos = prev.p2 + r1 + r2 + r3;
                    while (newPos > 10)
                    {
                        newPos -= 10;
                    }
                    int newScore = prev.s2 + newPos;
                    State newState = new State(prev.p1, newPos, prev.s1, newScore, true, newScore >= 21);
                    stateCount.AddToCount(newState, prevCount);
                }
            }
        }
    }
}


long p1Won = stateCount.Where(kv => kv.Key.won && kv.Key.s1 >= 21).Select(kv => kv.Value).Sum();
long p2Won = stateCount.Where(kv => kv.Key.won && kv.Key.s2 >= 21).Select(kv => kv.Value).Sum();

Console.WriteLine($"{Math.Max(p1Won, p2Won)}");

public record State(int p1, int p2, int s1, int s2, bool p1Active, bool won);
