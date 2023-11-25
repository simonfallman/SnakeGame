namespace SnakeTest;

class Program
{
    static void Main(string[] args)
    {
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
                new Coordinates(4, 4),
            };

            Snake snake = new Snake(grid, snakeCoordinates , moveRight);

            grid.GenerateEdibles(snake.Coordinates);


            do
            {
                if (IsGameOver(snake.Coordinates))
                {
                    Console.WriteLine("Game over, you hit yourself!");
                    break;
                }

                Console.Clear();
                grid.PrintGrid();
                Console.WriteLine($"Length: {snake.Coordinates.Count}");
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
                Thread.Sleep(200); // Adjust the sleep duration as needed

            } while (key.Key != ConsoleKey.Q);
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Game Over, you hit the wall");
        }
    }

    static bool IsGameOver(List<Coordinates> coordinates)
    {
        Coordinates newHead = coordinates[0];
        return coordinates.Skip(1).Any(coord => coord.X == newHead.X && coord.Y == newHead.Y);
    }
}

class Coordinates
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
class Snake
{
    private List<Coordinates> body;
    private Grid grid;
    private Movement currentMovement;

    public Snake(Grid grid, List<Coordinates> initialBody, Movement initialMovement)
    {
        this.grid = grid;
        this.body = initialBody;
        this.currentMovement = initialMovement;
    }

    public List<Coordinates> Coordinates => body;

    public void Move()
    {
        Coordinates newHead = currentMovement.ChangeDirection(body);

        if (!grid.IsOnEdible(newHead))
        {
            RemoveTail();
        }

        body.Insert(0, newHead);

        grid.SetCell(body);
        EatEdible();
    }

    private void RemoveTail()
    {
        Coordinates tail = body[body.Count - 1];
        grid.RemoveCell(tail);
        body.RemoveAt(body.Count - 1);
    }

    private void EatEdible()
    {
        Coordinates head = body[0];
        var eatenEdible = grid.Edibles.FirstOrDefault(e => e.X == head.X && e.Y == head.Y);

        if (eatenEdible != null)
        {
            grid.Edibles.Remove(eatenEdible);

            // Generate a new edible after eating
            grid.GenerateEdibles(body);
        }
    }

    public void SetMovement(Movement newMovement)
    {
        currentMovement = newMovement;
    }
}

interface IEdible
    {
        int X { get; }
        int Y { get; }
        public string GetEdibleSymbol()
        {
            return " X ";
        }
        
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
            return " 🍉";
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

        public string GetEdibleSymbol()
        {
            return " 🍌";
        }

    }

class Grid
{
    string[][] cells;
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
    public Grid(int sides) : this(sides, sides) {}

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

    public bool IsOnEdible(Coordinates position)
    {
        return Edibles.Any(edible => edible.X == position.X && edible.Y == position.Y);
    }

    public void GenerateEdibles(List<Coordinates> coordinates)
    {
            Random random = new Random();
            int row, column;
            bool isOccupiedBySnake;
            bool isOccupiedByEdible;

            if (coordinates.Count == (Rows * Columns))
            {
                HandleLastAppleScenario();
            }

            do
            {
                row = random.Next(0, Rows);
                column = random.Next(0, Columns);

                // Check if the generated coordinates are occupied by the snake
                isOccupiedBySnake = coordinates.Any(coord => coord.X == row && coord.Y == column);

                // Check if the generated coordinates are occupied by an existing edible
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
            int randomEdibleType = random.Next(4); // Assuming there are two types: 0 for Apple, 1 for Orange

            if (randomEdibleType == 0)
            {
                return new Apple(x, y);
            }
            else if (randomEdibleType == 1)
            {
                return new Banana(x, y);
            }
            else if (randomEdibleType == 2)
            {
                return new Melon(x, y);
            }
            else
            {
                return new Orange(x, y);
            }
        }
    private void HandleLastAppleScenario()
    {
        Console.WriteLine("You win!");
        Environment.Exit(0);
    }

}


abstract class Movement
{
    protected int XChange { get; }
    protected int YChange { get; }

    protected Movement(int xChange, int yChange)
    {
        XChange = xChange;
        YChange = yChange;
    }

    public abstract Coordinates ChangeDirection(List<Coordinates> coordinates);
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




