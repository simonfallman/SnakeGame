namespace SnakeGame;

public class Orange : Edible
{
    public Orange(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string GetEdibleSymbol()
    {
        return " 🍊";
    }
    public override void GetEatenBy(Snake snake)
    {
        snake.GrowByThree();
    }
}
