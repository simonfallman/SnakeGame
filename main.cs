using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame;

public class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start();
    }
}