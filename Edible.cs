namespace SnakeGame;

//1: Abstrakt klass
//2: Vi har en abstrakt klass som implementerar IEdible och som alla ätbara objekt ärver ifrån.
//3: Varför vi anvönt detta är att vissa metoder är gemensamma för all ätbara objekt såsom MoveEdibleToNewPosition och GetEatenBy har alla sin egen implementation.
public abstract class Edible : IEdible
{
    //1: Åtkomstmodifierare protected
    //2: Gör att klasser som ärver från Edible kan komma åt dessa variabler.
    //3: Vi har använt detta koncept för att de som ärver utav Edible ärver även X och Y koordinaterna. Dessa variabler är protected så de kan inte ändras utifrån superklass och subklass.
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public abstract void GetEatenBy(Snake snake);
    public abstract string GetEdibleSymbol();

    public void MoveEdibleToNewRandomPosition(Snake snake)
    {
        (int row, int column) = snake.Grid.AvailablePosition(snake.Coordinates);
        X = row;
        Y = column;
        snake.Grid.Edibles[0]= this;
        snake.Grid.Cells[row][column] = this.GetEdibleSymbol();
    }

}
