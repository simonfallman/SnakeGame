namespace SnakeGame;

//1: Interface
//2: Vi har ett interface IEdible som är kontraktet som subklasserna måste följa.
//3: Vi har använt detta koncept för att vi ska kunna skapa olika ätbara objekt som alla har olika implementationer. Därav möjliggör subtypspolyformism eftersom vi har olika beteenden för olika ätbara objekt. Vi säkerställer att alla ätbara objekt innehåller och ger sin egna implementation av metoderna i kontraktet.
public interface IEdible
{
    public int X { get; }
    public int Y { get; }
    string GetEdibleSymbol();
    void GetEatenBy(Snake snake);
    void MoveEdibleToNewRandomPosition(Snake snake);
}
