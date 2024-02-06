using OpenGL__Tutorial;
using System;

class Program
{
    static void Main()
    {
        using (Game game = new Game(1200, 1000, "LearnOpenTK"))
        {
            game.Run();
        }
    }
}
