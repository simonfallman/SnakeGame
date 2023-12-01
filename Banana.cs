namespace SnakeGame;

public class Banana : Edible
{
    public Banana(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string GetEdibleSymbol()
    {
        return " 🍌";
    }
    public override void GetEatenBy(Snake snake)
    {
        snake.CrazyMonkeyFriday();
    }
}
