namespace SnakeGame;

//1: Arv av klass
//2: Vi har en klass som ärver från Edible och implementerar IEdible.
//3: Vi har använt detta koncept för att vi ska kunna skapa olika ätbara objekt som alla har olika implementationer. Vi slipper upprepning av kod genom att ärva data och subtypspolyformism ärver vi beteenden. Alla frukter ärver samma metod MoveEdibleToNewPosition.
public class Apple : Edible
{
    public Apple(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string GetEdibleSymbol()
    {
        return " 🍎";
    }
    public override void GetEatenBy(Snake snake)
    {
        snake.GrowByOne();
    }
}
