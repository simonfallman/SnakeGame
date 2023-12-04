namespace SnakeGame;

public class Snake
{
    //1: Objektkomposition, Snake har en lista av koordinater, har då en has-a relation till Coordinates. Har också en has-a relation till Grid och Movement samt Game.
    //2: Klassen snake har en lista av koordinater som representerar ormens kropp. Den har också en instans av Grid och Movement.
    //3: Vi har använt detta koncept för att ormen ska kunna komma åt medlemmar i andra klasser som i flera krävs i Snake klassens medlemmar.
    private List<Coordinates> body;
    private Game game;
    public Grid Grid { get; private set; }
    private Movement currentMovement;
    //1: Beroendeinjektion - Konstruktorinjektion
    //2: Alla de olika klasserna som Snake tar in som parametrar i konstruktorn är beroenden till Snake klassen. Alltså injicerar vi beroenden till Snake klassen.
    //3: Vi har använt detta koncept för att urskilja Snakes skapning mot dess användning. Vi kan skicka in olika beroenden och Snake beter sig annorlunda.
    public Snake(Game game, Grid grid, List<Coordinates> initialBody, Movement initialMovement)
    {
        this.game = game;
        this.Grid = grid;
        this.body = initialBody;
        this.currentMovement = initialMovement;
    }

    public List<Coordinates> Coordinates => body;

    //1: Computed property
    //2: Propertyn räknar ut ditt score genom att multiplicera längden med 100.
    //3: Vi har använt detta koncept för att vid varje "tick" alltså varje gång vi kör vår loop i Game uppdateras scoren. Varför computed property? För att det ska kunna räknas ut automatiskt.
    public int Score => body.Count * 100;
    private long lastEdibleMoveTicks = DateTime.Now.Ticks;

    private bool ShouldMoveEdible()
    {
        long elapsedTicks = DateTime.Now.Ticks - lastEdibleMoveTicks;
        long elapsedSeconds = TimeSpan.FromTicks(elapsedTicks).Seconds;
        return elapsedSeconds > 7;
    }
    public void Move()
    {
        Coordinates newHead = currentMovement.ChangeDirection(body);
        IEdible currentEdible = Grid.CurrentEdibleOrNull(newHead);


        if (IsGameOver(newHead, body.Skip(1).ToList())) ///ChatGpt har använts för att förstå logiken bakom detta.
        {
            Console.WriteLine("Game over, you hit yourself!");
            game.AskToPlayAgain();
        }
        if(Grid.HasHitWall(newHead))
        {
            Console.WriteLine("Game over, you hit the wall!");
            game.AskToPlayAgain();
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
    //3: Vi har använt detta koncept så vi har möjlighet att enkelt ändra rörelsehastigeheten på ormen vid de olika move knapparna. Eller behålla default speed.
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
