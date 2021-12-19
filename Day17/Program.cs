int xMin = 79;
int xMax = 137;
int yMin = -176;
int yMax = -117;

bool withinTarget(Point pos)
{
    return pos.x >= xMin && pos.x <= xMax && pos.y >= yMin && pos.y <= yMax;
}

bool pastTarget(Point pos)
{
    return pos.y < yMin|| pos.x > xMax;
}

Point friction = new Point(-1, 0);
Point gravity = new Point(0, -1);

IEnumerable<Point> trajectory(Point initialVel)
{
    Point vel = initialVel;
    Point pos = new Point(0, 0);
    while (true)
    {
        pos = pos + vel;
        if (vel.x < 0)
        {
            vel = vel - friction;
        }
        else if (vel.x > 0)
        {
            vel = vel + friction;
        }
        vel = vel + gravity;
        yield return pos;
    }
}

int bestHeight = int.MinValue;
int goodVelocities = 0;

for (int x = 1; x <= xMax; ++x) // No need to consider starting velocities that take to the right of target in one step
{
    if (x * (x + 1) / 2 < xMin)
    {
        continue; // Skip x values where we don't reach target area at all
    }
    // vertical speeds outside of this range take us beyond the target either in step 1, or the first step after
    // gravity brings us back to 0 height (which it always does exactly).
    for (int y = yMin; y <= -yMin; ++y)
    {
        IEnumerable<Point> relevantTrajectory = trajectory(new Point(x, y)).TakeWhile(pos => !pastTarget(pos));
        if (relevantTrajectory.Any(pos => withinTarget(pos))) {
            bestHeight = Math.Max(bestHeight, relevantTrajectory.Max(pos => pos.y));
            goodVelocities++;
        }
    }
}

Console.WriteLine($"{bestHeight}");
Console.WriteLine($"{goodVelocities}");

public record Point(int x, int y)
{
    public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
}

