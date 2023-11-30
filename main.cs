using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeTest;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Press any key to start");
        Console.ReadKey();
        Console.Clear();
        ConsoleKeyInfo key = new ConsoleKeyInfo();
        try
        {
            Movement moveRight = new MoveRight();
            Movement moveLeft = new MoveLeft();
            Movement moveDown = new MoveDown();
            Movement moveUp = new MoveUp();
            Grid grid = new Grid(10);
            List<Coordinates> snakeCoordinates = new List<Coordinates>
            {
                new Coordinates(0, 0),
            };
            List<Coordinates> snakeCoordinates1 = new List<Coordinates>
            {
                new Coordinates(0, 5),
            };

            //1: Beroendeinjektion - Konstruktorinjektion
            //2: Snake-klassen tar in en instans av Grid och Movement genom konstruktorn.
            //3: Vi har använt detta koncept för att instantiera en Snake i main och sedan skicka in grid och movement i konstruktorn.
            Snake snake = new Snake(grid, snakeCoordinates, moveRight);

            grid.GenerateEdibles(snake.Coordinates);

            do
            {
                Console.Clear();
                grid.PrintGrid();

                if (grid.IsGridFull())
                {
                    Console.WriteLine("You win!");
                    break;
                }
                Console.WriteLine($"Length: {snake.Length}");
                Console.WriteLine("Press Q to quit");

                if (Console.KeyAvailable)
                {
                    key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            snake.SetMovement(moveUp);
                            break;
                        case ConsoleKey.DownArrow:
                            snake.SetMovement(moveDown);
                            break;
                        case ConsoleKey.LeftArrow:
                            snake.SetMovement(moveLeft, 300);
                            break;
                        case ConsoleKey.RightArrow:
                            snake.SetMovement(moveRight, 300);
                            break;
                    }
                }
                snake.Move();

            } while (key.Key != ConsoleKey.Q);
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Game Over, you hit the wall");
            Environment.Exit(0);
        }
    }

}

public class Coordinates
{
    public int X { get; set; }
    public int Y { get; set; }
    public Coordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
public class Snake
{
    //1: Objektkomposition, Snake har en lista av koordinater, har då en has-a relation till Coordinates. Har också en has-a relation till Grid och Movement.
    //2: Klassen snake har en lista av koordinater som representerar ormens kropp. Den har också en instans av Grid och Movement.
    //3: Vi har använt detta koncept för att ormen ska kunna röra sig och för att vi ska kunna skriva ut ormens kropp på griden.
    private List<Coordinates> body;
    public Grid Grid { get; private set;}
    private Movement currentMovement;
    //1: Beroendeinjektion - Konstruktorinjektion
    //2: Snake-klassen tar in en instans av Grid och Movement genom konstruktorn. Detta gör att vi kan skapa en grid och movement i main och sedan skicka in dessa i konstruktorn.
    //3: Vi har använt detta koncept för att vi ska kunna använda grid och movement i klassen för att 
    public Snake(Grid grid, List<Coordinates> initialBody, Movement initialMovement)
    {
        this.Grid = grid;
        this.body = initialBody;
        this.currentMovement = initialMovement;
    }

    public List<Coordinates> Coordinates => body;

    //1: Computed property
    //2: Propertyn räknar ut längden på ormen genom att räkna antalet element i listan body vid varje anrop.
    //3: Vi har använt detta koncept för att vi enkelt ska kunna få ut längden på ormen.
    public int Length => body.Count;
    private long lastEdibleMoveTicks = DateTime.Now.Ticks;

    private bool ShouldMoveEdible()
    {
        long elapsedTicks = DateTime.Now.Ticks - lastEdibleMoveTicks;
        long elapsedSeconds = TimeSpan.FromTicks(elapsedTicks).Seconds;
        return elapsedSeconds > 5;
    }
    public void Move()
    {
        Coordinates newHead = currentMovement.ChangeDirection(body);
        IEdible currentEdible = Grid.CurrentEdibleOrNull(newHead);


        if (IsGameOver(newHead, body.Skip(1).ToList())) ///ChatGpt har använts för att förstå logiken bakom detta.
        {
            Console.WriteLine("Game over, you hit yourself!");
            Environment.Exit(0);
        }

        body.Insert(0, newHead);
        RemoveTail();

        if (currentEdible != null)
        {
            Grid.Edibles.Remove(currentEdible);
            currentEdible.GetEatenBy(this);
            Grid.GenerateEdibles(body);
            lastEdibleMoveTicks = DateTime.Now.Ticks;
        }
        else if (currentEdible == null && ShouldMoveEdible())
        {
            if (Grid.Edibles.Count > 0)
            {
                Grid.RemoveCell(new Coordinates(Grid.Edibles[0].X, Grid.Edibles[0].Y));
                Grid.Edibles[0].MoveEdibleToNewRandomPosition(this);


                lastEdibleMoveTicks = DateTime.Now.Ticks;

            }
        }


        Grid.SetCell(body);
        Thread.Sleep(currentMovement.GetSpeed());
    }

    public void GrowByThree()
    {
        for (int i = 0; i < 3; i++)
        {
            Coordinates tail = body[body.Count - 1];
            body.Add(tail);
            Grid.SetCell(body);
        }
    }
    public void CrazyMonkeyFriday()
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 4);
        for (int i = 0; i < randomNumber; i++)
        {
            Thread.Sleep(100);
            Console.WriteLine("Crazy Monkey Friday!!!");
        }
    }

    private static bool IsGameOver(Coordinates newHead, List<Coordinates> body)
    {
        return body.Any(coord => coord.X == newHead.X && coord.Y == newHead.Y);
    }

    public void GrowByOne()
    {
        Coordinates tail = body[body.Count - 1];
        body.Add(tail);
        Grid.SetCell(body);

    }

    private void RemoveTail()
    {
        Coordinates tail = body[body.Count - 1];
        Grid.RemoveCell(tail);
        body.RemoveAt(body.Count - 1);
    }
    //1: Overloading av instansmetoder
    //2: Vi kan skicka in olika parametrar till metoden SetMovement och den kommer att anropa olika konstruktorer beroende på vilka parametrar vi skickar in.
    //3: Vi har använt detta koncept så vi har möjlighet att enkelt ändra röreslehastigeheten på ormen vid olika knapptryckningar. Elelr behålla default speed.
    public void SetMovement(Movement newMovement)
    {
        currentMovement = newMovement;
    }
    public void SetMovement(Movement newMovement, int speed)
    {
        currentMovement = newMovement;
        currentMovement.SetSpeed(speed);

    }
}
//1: Interface
//2: Vi har ett interface IEdible som är kontraktet som subklasserna måste följa.
//3: Vi har använt detta koncept för att vi ska kunna skapa olika ätbara objekt som alla har olika implementationer. Därav möjliggör subtypspolyformism eftersom vi har olika beteenden för olika ätbara objekt.
public interface IEdible
{
    public int X { get; }
    public int Y { get; }
    string GetEdibleSymbol();
    void GetEatenBy(Snake snake);

    void MoveEdibleToNewRandomPosition(Snake snake);
}
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
//1: Arv av klass
//2: Vi har en klass som ärver från Edible och implementerar IEdible.
//3: Vi har använt detta koncept för att vi ska kunna skapa olika ätbara objekt som alla har olika implementationer. Vi slipper upprepning av kod genom att ärva data och subtypspolyformism ärver vi beteenden.
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

public class Grid
{
    //1: Inkapsling / Informationsgömning, gör att vi inte kan ändra på gridens storlek efter att den har skapats.
    //2: Vi har fältet cells som private.
    //3: Vi har valt att använda detta koncept för att man ej ska kunna ändra cells utifrån Grid klassen.
    private string[][] cells;
    public string[][] Cells => cells;
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    private List<IEdible> edibles = new List<IEdible>();
    public List<IEdible> Edibles => edibles;


    public Grid(int rows, int columns)
    {
        this.Rows = rows;
        this.Columns = columns;
        cells = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            cells[i] = new string[columns];
            for (int j = 0; j < columns; j++)
            {
                cells[i][j] = " . ";
            }
        }
    }
    //1: Overloading av konstruktorer
    //2: Beroende på om vi skriver vår grid med ett eller 2 tal så kommer den att skapa en kvadratisk grid eller en rektangulär grid.
    //3: Vi har använt detta koncept så vi enkelt kan välja formatet på vår grid. Ger oss mer flexibilitet.
    //1: Konstruktor-kedjning
    //2: Vi använder nyckelordet this, för att kalla på den andra konstruktorn.
    //3: Vi har använt detta koncept för att minimera duplicering av kod, konstruktor-kedjning gör koden mer läsbar och lättare att underhålla.
    public Grid(int sides) : this(sides, sides) { }

    public void PrintGrid()
    {
        foreach (var row in cells)
        {
            foreach (var cell in row)
            {
                Console.Write(cell);
            }
            Console.WriteLine();
        }
    }
    public void RemoveCell(Coordinates position)
    {
        cells[position.X][position.Y] = " . ";
    }

    public void SetCell(List<Coordinates> coordinates)
    {
        foreach (var coord in coordinates)
        {
            cells[coord.X][coord.Y] = " 🐍";
        }
    }

    public IEdible CurrentEdibleOrNull(Coordinates position)
    {
        return Edibles.FirstOrDefault(edible => edible.X == position.X && edible.Y == position.Y);
    }
    public (int, int) AvailablePosition(List<Coordinates> snake)
    {
        Random random = new Random();
        int row, column;
        bool isOccupiedBySnake;
        bool isOccupiedByEdible;

        do
        {
            row = random.Next(0, Rows);
            column = random.Next(0, Columns);

            isOccupiedBySnake = snake.Any(coord => coord.X == row && coord.Y == column);

            isOccupiedByEdible = edibles.Any(edible => edible.X == row && edible.Y == column);

        } while (isOccupiedBySnake || isOccupiedByEdible);

        return (row, column);

    }

    public void GenerateEdibles(List<Coordinates> snake)
    {
        do
        {
            //1: Subtypspolymorfism
            //2: Vi skapar en instans av en klass som implementerar IEdible och sedan lägger vi till den i vår lista av ätbara objekt. Spelar ingen roll vilket ätbart objekt det är, utan vi vet att den uppfyller IEdible kontraktet.
            //3: Vi har använt detta koncept för att oberoende på vilken subtyp det är så behandlar vi det som en IEdible. Detta gör att vi kan skapa olika ätbara objekt som alla har olika beteenden.
            (int row, int column) = AvailablePosition(snake);
            IEdible randomEdible = CreateRandomEdible(row, column);
            edibles.Add(randomEdible);
            cells[row][column] = randomEdible.GetEdibleSymbol();

        } while (edibles.Count < 1);
    }
public IEdible CreateRandomEdible(int x, int y)
{
    Random random = new Random();
    int randomEdibleType = random.Next(11);

    if (randomEdibleType <= 5)
    {
        return new Apple(x, y);
    }
    else if (randomEdibleType >= 6 && randomEdibleType < 9)
    {
        return new Orange(x, y);
    }
    else
    {
        return new Banana(x, y);
    }
}
public bool IsGridFull()
{
    foreach (var row in cells)
    {
        foreach (var cell in row)
        {
            if (cell == " . ")
            {
                return false;
            }
        }
    }
    return true;
}

}
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

class Move : Movement
{
    public Move(int xChange, int yChange) : base(xChange, yChange) { }

    public override Coordinates ChangeDirection(List<Coordinates> coordinates)
    {
        Coordinates head = coordinates[0];
        return new Coordinates(head.X + XChange, head.Y + YChange);
    }
}

class MoveRight : Move
{
    public MoveRight() : base(0, 1) { }
}

class MoveLeft : Move
{
    public MoveLeft() : base(0, -1) { }
}

class MoveDown : Move
{
    public MoveDown() : base(1, 0) { }
}

class MoveUp : Move
{
    public MoveUp() : base(-1, 0) { }
}




