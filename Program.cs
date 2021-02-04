using System;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static void Main()
        {
            GameMenu.GenerateGameMenu();
        }
    }

    class SnakeGame
    {
        readonly GameWorld game = GameWorld.GameWorldInstance;
        private bool InitialStart = true;
        bool IsGameRunning { get; set; } = true;
        PlayerController controller;
        readonly Timer Timer = Timer.TimerInstance;
        private Vector2D[] ArenaSize { get; set; }
        private int Difficulty { get; set; }

        /// <summary>
        /// Start a new fresh game with the passsed in difficulty and arena size
        /// </summary>
        /// <param name="arenaSize">A vector2D array representing top left corner, and bottom right corner</param>
        /// <param name="difficulty">An int representing the difficult, this manipulates amount of portals as walls</param>
        public void StartGame(Vector2D[] arenaSize, int difficulty)
        {
            ArenaSize = arenaSize;
            Difficulty = difficulty;
            IsGameRunning = true;

            Console.CursorVisible = false;
            Console.Clear();

            // Generate the arena, and then set the same size of the arena to the console window
            game.GenerateArena(ArenaSize, Difficulty);
            #pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(ArenaSize[1].Y + 1, ArenaSize[1].X + 1); // + 1 must be here, since a console.Write will put the cursor one step outside the arena bounds 
            Console.SetBufferSize(ArenaSize[1].Y + 1, ArenaSize[1].X + 1);

            // Create a new player at the center of the arena
            Player player = new(Environment.MachineName, new Vector2D((ArenaSize[0].X + ArenaSize[1].X) / 2, (ArenaSize[0].Y + ArenaSize[1].Y) / 2));
            game.GameObjects.Add(player);
            controller = new PlayerController(player);

            // WorlUpdate will update the gameboard. Render all the gameObjects
            Timer.WorldUpdate();

            // Render the walls. Will only need to render once every game start
            game.RenderWalls();

            // This is the main loop of the program
            while (IsGameRunning)
            {
                // Start a task that will detect what key you are pressing asynchrounosly
                Task<char> detectedKey = DetectKeyPress();
                char keyPress = detectedKey.Result;

                // Get the new direction of the player.
                // With the beauty of a switch expression
                if (keyPress != '\0')
                {
                    Direction newDirection = keyPress switch
                    {
                        'W' => Direction.North,
                        'D' => Direction.East,
                        'S' => Direction.South,
                        'A' => Direction.West,
                        _ => Direction.None
                    };

                    controller.ChangeDirection(newDirection);
                }
            }
        }

        /// <summary>
        /// Asynchrounous detection of key pressing.
        /// Will return the Uppercase version of pressed letter, or the number, or other character.
        /// </summary>
        /// <returns>Returns the pressed key as a char</returns>
        private Task<char> DetectKeyPress()
        {
            char pressedKey = '\0';

            // Console.ReadKey() is a method that blocks every input and output to the console, this means that even if you are running
            // a multithreaded console program, your console will be blocked until a keypress is detected
            // Console.KeyAvailable will make sure that if you press a key, it will block only until the press is completed.
            // Console.KeyAvailable is a property of the Console class that is set to true if a keypress is iniciated
            while(Console.KeyAvailable)
            {
                // Convert the read key press to a string to see if the player presses the Escape key
                string readKey = Console.ReadKey(true).Key.ToString();
                if (readKey == "Escape")
                {
                    // Everything inside a Task.Run will be done synchronously
                    return Task.Run(() =>
                    {
                        // Reset the game
                        InitialStart = true;
                        game.ResetGame();
                        IsGameRunning = false;
                        Console.Clear();
                        return '\0';
                    });
                }

                pressedKey = readKey[0];

                if (InitialStart)
                {
                    Timer.GameTimeTimer();
                    InitialStart = false;
                }
            }

            return Task.Run(() => pressedKey);
        }
    }
}
