namespace SnakeGame;

public class Move : Movement
{
    public Move(int xChange, int yChange) : base(xChange, yChange) { }

    public override Coordinates ChangeDirection(List<Coordinates> coordinates)
    {
        Coordinates head = coordinates[0];
        return new Coordinates(head.X + XChange, head.Y + YChange);
    }
}
