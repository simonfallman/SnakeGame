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
            //1: Beroendeinjektion - Konstruktorinjektion
            //2: Snake-klassen tar in en instans av Grid och Movement genom konstruktorn.
            //3: Vi har använt detta koncept för att instantiera en Snake i main och sedan skicka in grid och movement i konstruktorn.
            Grid grid = new Grid(10);
            List<Coordinates> snakeCoordinates = new List<Coordinates>
            {
                new Coordinates(0, 0),
            };

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
                            snake.SetMovement(moveLeft, 200);
                            break;
                        case ConsoleKey.RightArrow:
                            snake.SetMovement(moveRight, 200);
                            break;
                    }
                }
                snake.Move();

            } while (key.Key != ConsoleKey.Q);
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Game Over, you hit the wall");
            Main(args);
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
    private Grid grid;
    private Movement currentMovement;
    //1: Beroendeinjektion - Egenskapsinjektion
    //2: Snake-klassen tar in en instans av Grid och Movement genom konstruktorn. Detta gör att vi kan skapa en grid och movement i main och sedan skicka in dessa i konstruktorn.
    //3: Vi har anvönt detta koncept för att vi ska kunna använda grid och movement i klassen för att konfiguera en Snake oberoende på hur de defineras.
    public Snake(Grid grid, List<Coordinates> initialBody, Movement initialMovement)
    {
        this.grid = grid;
        this.body = initialBody;
        this.currentMovement = initialMovement;
    }

    public List<Coordinates> Coordinates => body;

    //1: Computed property
    //2: Propertyn räknar ut längden på ormen genom att räkna antalet element i listan body vid varje anrop.
    //3: Vi har använt detta koncept för att vi enkelt ska kunna få ut längden på ormen.
    public int Length => body.Count;




    public void Move()
    {
        Coordinates newHead = currentMovement.ChangeDirection(body);
        IEdible currentEdible = grid.CurrentEdibleOrNull(newHead);


        if (IsGameOver(newHead, body.Skip(1).ToList())) ///ChatGpt har använts för att förstå logiken bakom detta.
        {
            Console.WriteLine("Game over, you hit yourself!");
            Environment.Exit(0);
        }

        body.Insert(0, newHead);
        RemoveTail();

        if (currentEdible != null)
        {
            grid.Edibles.Remove(currentEdible);
            currentEdible.GetEatenBy(this);
            grid.GenerateEdibles(body);
        }

        grid.SetCell(body);
        Thread.Sleep(currentMovement.GetSpeed());
    }

    public void GrowByThree()
    {
        for (int i = 0; i < 3; i++)
        {
            Coordinates tail = body[body.Count - 1];
            body.Add(tail);
            grid.SetCell(body);
        }
    }
    public void CrazyMonkeyFriday()
    {
        Random random = new Random();
        int randomNumber = random.Next(1, 5);
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
        grid.SetCell(body);

    }

    private void RemoveTail()
    {
        Coordinates tail = body[body.Count - 1];
        grid.RemoveCell(tail);
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
    int X { get; }
    int Y { get; }

    //1: Default interface method
    //2: Vi har en default implementation av metoden GetEdibleSymbol som alla klasser som implementerar IEdible har tillgång till.
    //3: För att om vi vill ändra symbolen för alla default implementationer så behöver vi bara ändra på en plats. Förhindrar kodupprepning.
    string GetEdibleSymbol()
    {
        return " X ";
    }
    void GetEatenBy(Snake snake);

}

public class Apple : IEdible
{
    public int X { get; }
    public int Y { get; }

    public Apple(int x, int y)
    {
        X = x;
        Y = y;
    }
    public string GetEdibleSymbol()
    {
        return " 🍎";
    }
    public void GetEatenBy(Snake snake)
    {
        snake.GrowByOne();
    }
}

public class Orange : IEdible
{
    public int X { get; }
    public int Y { get; }

    public Orange(int x, int y)
    {
        X = x;
        Y = y;
    }
    public string GetEdibleSymbol()
    {
        return " 🍊";
    }
    public void GetEatenBy(Snake snake)
    {
        snake.GrowByThree();
    }
}

public class Banana : IEdible
{
    public int X { get; }
    public int Y { get; }

    public Banana(int x, int y)
    {
        X = x;
        Y = y;
    }
    public string GetEdibleSymbol()
    {
        return " 🍌";
    }
    public void GetEatenBy(Snake snake)
    {
        snake.CrazyMonkeyFriday();
    }
}
public class Melon : IEdible
{
    public int X { get; }
    public int Y { get; }

    public Melon(int x, int y)
    {
        X = x;
        Y = y;
    }
    public void GetEatenBy(Snake snake)
    {
        snake.GrowByOne();
    }
}
public class Kiwi : IEdible
{
    public int X { get; }
    public int Y { get; }

    public Kiwi(int x, int y)
    {
        X = x;
        Y = y;
    }
    public void GetEatenBy(Snake snake)
    {
        snake.GrowByOne();
    }
}


public class Grid
{
    //1: Inkapsling / Informationsgömning, gör att vi inte kan ändra på gridens storlek efter att den har skapats.
    //2: Vi har fältet cells som private.
    //3: Vi har valt att använda detta koncept för att man ej ska kunna nå cells utifrån Grid klassen.
    private string[][] cells;
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

    public void GenerateEdibles(List<Coordinates> coordinates)
    {
        Random random = new Random();
        int row, column;
        bool isOccupiedBySnake;
        bool isOccupiedByEdible;

        do
        {
            row = random.Next(0, Rows);
            column = random.Next(0, Columns);

            isOccupiedBySnake = coordinates.Any(coord => coord.X == row && coord.Y == column);

            isOccupiedByEdible = edibles.Any(edible => edible.X == row && edible.Y == column);

            if (!isOccupiedBySnake && !isOccupiedByEdible)
            {
                //1: Subtypspolymorfism
                //2: Vi skapar en instans av en klass som implementerar IEdible och sedan lägger vi till den i vår lista av ätbara objekt. Spelar ingen roll vilket ätbart objekt det är, utan vi vet att den uppfyller IEdible kontraktet.
                //3: Vi har använt detta koncept för att oberoende på vilken subtyp det är så behandlar vi det som en IEdible. Detta gör att vi kan skapa olika ätbara objekt som alla har olika beteenden.
                IEdible randomEdible = CreateRandomEdible(row, column);
                edibles.Add(randomEdible);
                cells[row][column] = randomEdible.GetEdibleSymbol();
            }

        } while (isOccupiedBySnake || isOccupiedByEdible);
    }
    public IEdible CreateRandomEdible(int x, int y)
    {
        Random random = new Random();
        int randomEdibleType = random.Next(10);

        if (randomEdibleType <= 6)
        {
            return new Apple(x, y);
        }
        else if (randomEdibleType >= 7 && randomEdibleType < 9)
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
    //1: Åtkomstmodifierare protected
    //2: Gör att klasser som ärver från Movement kan komma åt dessa variabler. 
    //3: För att inkapsla så att dessa inte kan ändras utifrån klassträdet, alltså superklassen med dess subklasser. 
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
    //1: Konstruktor-kedjning
    //2: Vi använder nyckelordet base, för att kalla på superklassens konstruktor i subklassen.
    //3: Vi har använt detta koncept för att minimera duplicering av kod, konstruktor-kedjning gör koden mer läsbar och lättare att underhålla.
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




