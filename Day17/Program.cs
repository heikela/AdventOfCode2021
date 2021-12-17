// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Point bottomLeft = new Point(79, -176);
Point topRight = new Point(137, -117);

Boolean withinTarget(Point pos)
{
    return pos.x >= bottomLeft.x && pos.x <= topRight.x && pos.y >= bottomLeft.y && pos.y <= topRight.y;
}

Boolean pastTarget(Point pos)
{
    return pos.y < -176 || pos.x > 137;
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

for (int x = 10; x < 138; ++x)
{
    for (int y = -176; y < 177; ++y)
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

