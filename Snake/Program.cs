using System;
using System.Threading;
using System.Threading.Tasks;

/* 
 * Andreas Kvist Syned20Jon
 * 
 * Jag kan ha tänk fel med X och Y axeln med Console.SeCursorPosition och gjort det omvänt.
 * Well, det funkar som det ska :)
 */

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
        private readonly GameWorld game = GameWorld.GameWorldInstance;
        private readonly GameUpdateTimer Timer = GameUpdateTimer.TimerInstance;
        private readonly ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;
        private bool InitialStart { get; set; } = true;
        private bool IsGameRunning { get; set; } = true;
        private Vector2D[] ArenaSize { get; set; }

        private void SetupArena()
        {
#if DEBUG
            int debugReleaseExtraX = 1; // This is a variable to set the window and buffersize depening if it is debug or release. 
#else
            int debugReleaseExtraX = 2;
#endif

            // Generate the arena, and then set the same size of the arena to the console window
            game.GenerateArena(ArenaSize, ScoreBoard.Difficulty);
#pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(ArenaSize[1].Y + 1, ArenaSize[1].X + debugReleaseExtraX); // + 1 must be here, since a console.Write will put the cursor one step outside the arena bounds 
            Console.SetBufferSize(ArenaSize[1].Y + 1, ArenaSize[1].X + debugReleaseExtraX);
        }

        /// <summary>
        /// Start a new fresh game with the passsed in difficulty and arena size
        /// </summary>
        /// <param name="arenaSize">A vector2D array representing top left corner, and bottom right corner</param>
        /// <param name="difficulty">An int representing the difficult, this manipulates amount of portals as walls</param>
        public void StartGame(Vector2D[] arenaSize, int difficulty, string playerName)
        {
            ArenaSize = arenaSize;
            ScoreBoard.Difficulty = difficulty;
            IsGameRunning = true;

            Console.CursorVisible = false;
            Console.Clear();
            // Generate tha arena, and size down/up the console windows size
            SetupArena();

            // Create a new player at the center of the arena
            Player player = new(playerName ?? "player", new Vector2D((ArenaSize[0].X + ArenaSize[1].X) / 2, (ArenaSize[0].Y + ArenaSize[1].Y) / 2));
            game.CollisionObjects.Add(player);
            PlayerController controller = new(player);
            
            Timer.WorldUpdate(); // WorlUpdate will update the gameboard. Render all the gameObjects
            game.RenderWalls();  // Render the walls. Will only need to render once every game start

            // This loop is used for controlling the direction of the player
            while (GameWorld.IsThisBoolTrue(IsGameRunning))
            {
                // Start a task that will detect what key you are pressing asynchrounosly
                Task<string> detectedKey = DetectKeyPress();
                string keyPress = detectedKey.Result;

                // Get the new direction of the player.
                // With the beauty of a switch expression
                if (keyPress != null)
                {
                    Direction newDirection = keyPress switch
                    {
                        "W" or "UpArrow" => Direction.North,
                        "D" or "RightArrow" => Direction.East,
                        "S" or "DownArrow" => Direction.South,
                        "A" or "LeftArrow" => Direction.West,
                        _ => player.Direction
                    };

                    controller.ChangeDirection(newDirection);
                }
            }
        }

        /// <summary>
        /// Resets the game to factory defaults
        /// </summary>
        public void ResetGame()
        {
            Player player = (Player)game.CollisionObjects.Find((x) => x.GetObjectType() == ObjectType.Player);
            PlayerScore playerScore = new(player.GetIdentity(), ScoreBoard.Score, ScoreBoard.PlayTime, ScoreBoard.Difficulty);
            ScoreBoard.Score = 0;
            ThreadPool.QueueUserWorkItem((x) => HighScore.HighScoreInstance.UpdateHighScore(playerScore));
            Console.Clear();
            Timer.GameHasEnded = true;
            Timer.GameIsEnding = true;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("Returning to the game menu...");
            // Thread.Sleep will block the current thread x amount of milliseconds
            Thread.Sleep(1200);
            game.CollisionObjects.Clear();
        }

        /// <summary>
        /// Asynchrounous detection of key pressing.
        /// Will return the Uppercase version of pressed letter, or the number, or other character.
        /// </summary>
        /// <returns>Returns the pressed key as a string</returns>
        private Task<string> DetectKeyPress()
        {
            string readKey = null;

            // Console.ReadKey() is a method that blocks every input and output to the console, this means that even if you are running
            // a multithreaded console program, your console will be blocked until a keypress is detected
            // Console.KeyAvailable will make sure that if you press a key, it will block only until the press is completed.
            // Console.KeyAvailable is a property of the Console class that is set to true if a keypress is iniciated
            while (GameWorld.IsThisBoolTrue(Console.KeyAvailable))
            {
                // Convert the read key press to a string to see if the player presses the Escape key
                readKey = Console.ReadKey(true).Key.ToString();
                if (readKey == "Escape")
                {
                    // Everything inside a Task.Run will be done synchronously
                    return Task.Run(() =>
                    {
                        // Reset the game
                        InitialStart = true;
                        ResetGame();
                        IsGameRunning = false;
                        Console.Clear();
                        return (string)null;
                    });
                }

                // If it is the start of a new game. Start the game timer
                if (GameWorld.IsThisBoolTrue(InitialStart))
                {
                    Timer.GameTimeTimer();
                    InitialStart = false;
                }
            }

            return Task.Run(() => readKey);
        }
    }
}