namespace SnakeGame;

public class Game
{
    private Snake snake;
    private Grid grid = new Grid(10);

    public Game()
    {
        snake = new Snake(this, this.grid, new List<Coordinates> { new Coordinates(0, 0) }, new MoveRight());
        grid.GenerateEdibles(snake.Coordinates);
    }

    public void Start()
    {
        Console.WriteLine(" _____                _          ");
        Console.WriteLine("/  ___|              | |         ");
        Console.WriteLine("\\ `--.  _ __    __ _ | | __  ___ ");
        Console.WriteLine(" `--. \\| '_ \\  / _` || |/ / / _ \\");
        Console.WriteLine("/\\__/ /| | | || (_| ||   < |  __/");
        Console.WriteLine("\\____/ |_| |_| \\__,_||_|\\_\\ \\___|");
        Console.WriteLine("Press any key to start");
        Console.ReadKey();
        Console.Clear();
        Play();
    }

    private void Play()
    {
        ConsoleKeyInfo key = new ConsoleKeyInfo();

        do
        {
            Console.Clear();
            grid.PrintGrid();

            if (grid.IsGridFull())
            {
                Console.WriteLine("You win!");
                break;
            }

            Console.WriteLine($"Score💀: {snake.Score}");
            Console.WriteLine("Press Q to quit");

            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                HandleKeyPress(key);
            }

            snake.Move();

        } while (key.Key != ConsoleKey.Q);

        AskToPlayAgain();
    }

    private void HandleKeyPress(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                snake.SetMovement(new MoveUp());
                break;
            case ConsoleKey.DownArrow:
                snake.SetMovement(new MoveDown());
                break;
            case ConsoleKey.LeftArrow:
                snake.SetMovement(new MoveLeft(), 300);
                break;
            case ConsoleKey.RightArrow:
                snake.SetMovement(new MoveRight(), 300);
                break;
        }
    }

    public void AskToPlayAgain()
    {
        Console.WriteLine("Do you want to play again? (Y/N)");

        ConsoleKeyInfo response = Console.ReadKey();
        if (response.Key == ConsoleKey.Y)
        {
            Console.Clear();
            Restart();
        }
        else
        {
            Console.WriteLine("Thanks for playing!");
            Environment.Exit(0);
        }
    }

    private void Restart()
    {
        Game newGame = new Game();
        newGame.Start();
    }
}

