namespace SnakeGame;

public class Grid
{
    //1: Inkapsling / Informationsgömning, gör att vi inte kan ändra på gridens storlek efter att den har skapats.
    //2: Vi har fältet cells som private.
    //3: Vi har valt att använda detta koncept för att man ej ska kunna ändra cells utifrån Grid klassen. Detta gör vi för att säkerställa att cells inte ändras utifrån Grid klassen.
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
    //3: Vi har använt detta koncept så vi enkelt kan välja formatet på vår grid. Ger oss mer flexibilitet. Vi som programmerare kan välja vilken grid vi vill ha. Inte slutanvändaren.
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
    public bool HasHitWall(Coordinates snake)
        {
            return snake.X < 0 || snake.X >= Rows || snake.Y < 0 || snake.Y >= Columns;
        }

    public void GenerateEdibles(List<Coordinates> snake)
    {
        do
        {
            (int row, int column) = AvailablePosition(snake);
            //1: Subtypspolymorfism
            //2: Vi skapar en instans av en klass som implementerar IEdible och sedan lägger vi till den i vår lista av ätbara objekt. Spelar ingen roll vilket ätbart objekt det är, utan vi vet att den uppfyller IEdible kontraktet.
            //3: Vi har använt detta koncept för att oberoende på vilken subtyp det är så behandlar vi det som en IEdible. Detta gör att vi kan skapa olika ätbara objekt som alla har olika beteenden.
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
