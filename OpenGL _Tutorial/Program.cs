using OpenGL__Tutorial;
using System;

class Program
{
    static void Main()
    {
        using (Game game = new Game(800, 600, "LearnOpenTK"))
        {
            game.Run();
        }
    }
}
