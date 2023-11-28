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
            //Subtypspolymorfism, vi kan skapa en instans av en klass som ärver från en annan klass och sedan använda den som om den vore av basklassens typ.
            Movement moveRight = new MoveRight();
            Movement moveLeft = new MoveLeft();
            Movement moveDown = new MoveDown();
            Movement moveUp = new MoveUp();
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
                            snake.SetMovement(moveLeft);
                            break;
                        case ConsoleKey.RightArrow:
                            snake.SetMovement(moveRight);
                            break;
                    }
                }

                snake.Move();
                Thread.Sleep(300); // Adjust the sleep duration as needed
                // Inside your game loop


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
    private List<Coordinates> body;
    private Grid grid;
    private Movement currentMovement;
    // Beroendeinjektion: Snake-klassen tar in en instans av Grid och Movement genom konstruktorn. Detta gör att vi kan skapa en grid och movement i main och sedan skicka in dessa i konstruktorn.
    public Snake(Grid grid, List<Coordinates> initialBody, Movement initialMovement)
    {
        this.grid = grid;
        this.body = initialBody;
        this.currentMovement = initialMovement;
    }

    public List<Coordinates> Coordinates => body;

    //Computed property, returnerar längden på ormen.
    public int Length => body.Count;




    public void Move()
    {
        Coordinates newHead = currentMovement.ChangeDirection(body);
        IEdible currentEdible = grid.CurrentEdibleOrNull(newHead);


        if (IsGameOver(newHead, body.Skip(1).ToList()))
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
    }

    public void GrowByThree()
    {
        // Assuming each element of the body represents one unit
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

    public void SetMovement(Movement newMovement)
    {
        currentMovement = newMovement;
    }
}
//Interface för ätbara objekt, alla ätbara objekt måste ha en X och Y koordinat. Ger möjlighet till subtypspolymorfism genom att lätt kunna addera nya frukter som uppfyller kontraktet.
public interface IEdible
{
    int X { get; }
    int Y { get; }

    //Default interface method, Subklassen kan ha egen implementation, men om de inte har det så returneras denna.
    string GetEdibleSymbol()
    {
        return " X ";
    }
    void GetEatenBy(Snake snake);

}
public abstract class Edible : IEdible
{
    public int X { get; }
    public int Y { get; }

    protected Edible(int x, int y)
    {
        X = x;
        Y = y;
    }
    public abstract string GetEdibleSymbol();


    public void GetEatenBy(Snake snake)
    {
        WhenEatenBy(snake);
    }
    protected abstract void WhenEatenBy(Snake snake);
}

public class Apple : Edible
{
    public Apple(int x, int y) : base(x, y) { }

    public override string GetEdibleSymbol()
    {
        return " 🍎";
    }

    protected override void WhenEatenBy(Snake snake)
    {
        snake.GrowByOne();
    }
}

public class Orange : Edible
{
    public Orange(int x, int y) : base(x, y) { }

    public override string GetEdibleSymbol()
    {
        return " 🍊";
    }

    protected override void WhenEatenBy(Snake snake)
    {
        snake.GrowByThree();
    }
}

public class Banana : Edible
{
    public Banana(int x, int y) : base(x, y) { }

    public override string GetEdibleSymbol()
    {
        return " 🍌";
    }

    protected override void WhenEatenBy(Snake snake)
    {
        snake.CrazyMonkeyFriday();
    }
}


public class Grid
{
    //Inkapsling / Informationsgömning, gör att vi inte kan ändra på gridens storlek efter att den har skapats.
    private string[][] cells;
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    //Objektkompistion, grid har en lista av ätbara objekt, har då en has-a relation till IEdible.
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
    //Overloading av konstruktorer, Beroende på om vi skriver vår grid med ett eller 2 tal så kommer den att skapa en kvadratisk grid eller en rektangulär grid.
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

//Abstract klass som alla rörelser ärver från, overridar abstract metoden ChangeDirection som alla rörelser måste ha.
public abstract class Movement
{
    //Åtkomstmodifierare protected, gör att klasser som ärver från Movement kan komma åt dessa variabler.
    protected int XChange { get; }
    protected int YChange { get; }

    protected Movement(int xChange, int yChange)
    {
        XChange = xChange;
        YChange = yChange;
    }

    public abstract Coordinates ChangeDirection(List<Coordinates> coordinates);
}
//Arv av klasser, alla rörelser ärver från Move som i sin tur ärver från Movement. Move är en konkret klass som implementerar ChangeDirection metoden.

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
    //Konstruktor-kedjning som anropar basklassens konstruktor med värdena baserat på vad vi skriver i base parantesen.
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




