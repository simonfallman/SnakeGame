namespace SnakeGame;

public abstract class Movement
{
    protected int XChange { get; }
    protected int YChange { get; }

    protected Movement(int xChange, int yChange)
    {
        XChange = xChange;
        YChange = yChange;
    }

    public abstract Coordinates ChangeDirection(List<Coordinates> coordinates);
    private int speed = 300;

    public virtual void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public int GetSpeed()
    {
        return speed;
    }
}
