using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    /* TODO
     * Add a menu screen.   check
     * 
     * Start new game   check
     *      Choose difficulty   check
     *      
     * Exit game    check
     * Credits      check
     */
    public struct GameMenu
    {
        private static Action CallChoosenMethod;    // This delegate is used to keep the code more neat and clean

        public static void GenerateGameMenu()
        {
            Console.SetCursorPosition(0, 0);

            while (true)
            {
                Console.CursorVisible = true;
                int choosenOption = ShowGameMenu();

                // Depening of what the user chose, call said method
                CallChoosenMethod = choosenOption switch
                {
                    1 => StartNewGame,
                    2 => RollCredits,
                    3 => ExitGame,
                    _ => ExitGame   // Not actually nessecary. But removes annoying warning
                };

                CallChoosenMethod();
            }

            //Timer Timer = Timer.TimerInstance;
            //Console.Write($"Played for {Timer.GameTime} seconds");
        }

        /// <summary>
        /// Show the game menu.
        /// </summary>
        /// <returns>An int between 1 and 3 (inclusive) that represents the choosen value</returns>
        private static int ShowGameMenu()
        {
            Console.Clear();
            Console.WriteLine("(1) : New Game");
            Console.WriteLine("(2) : Credits");
            Console.WriteLine("(3) : Exit game");

            int? ChoosenOption = 0;
            bool validInput;
            do
            {
                Console.Write(ChoosenOption == null ? "Not a valid input. 1 - 3: " : "\r");
                string userChoice = Console.ReadLine();

                ChoosenOption = userChoice switch
                {
                    "1" => 1,
                    "2" => 2,
                    "3" => 3,
                    _ => null
                };

                validInput = ChoosenOption != null;
            } while (!validInput);

            return (int)ChoosenOption;
        }

        /// <summary>
        /// Reset the game settings and start a new game
        /// </summary>
        private static void StartNewGame()
        {
            Timer timer = Timer.TimerInstance;
            timer.GameTime = 0;
            timer.GameEnd = false;
            // Tuples!!!!
            (Vector2D[] arenaSize, int difficulty) = ChoosenDifficulty();

            SnakeGame game = new();
            game.StartGame(arenaSize, difficulty);   // Start the game
        }

        /// <summary>
        /// Returns a Vector2D array and an interger in the range of 1 to 3 (inclusive)
        /// </summary>
        /// <returns>A Vector2D array representing the arena size and an interger that represents the difficulty choosen</returns>
        private static (Vector2D[], int) ChoosenDifficulty()
        {
            Console.Clear();
            Console.WriteLine("(1) : Easy");    // Only portal walls            (4, 0) --> (20, 35)
            Console.WriteLine("(2) : Medium");  // Few portal walls             (4, 0) --> (15, 30)
            Console.WriteLine("(3) : Hard");    // Only walls, no portal walls  (4, 0) --> (15, 30)

            Vector2D[] arenaSize;
            int? difficulty = 0;
            bool validInput;
            do
            {
                Console.Write(difficulty == null ? "Not a valid input. 1 - 3: " : "\r");
                string input = Console.ReadLine();
                // TUUUUUUUPLES!
                (arenaSize, difficulty) = input switch
                {
                    "1" => (new Vector2D[] { new(4, 0), new(20, 35) }, 1),
                    "2" => (new Vector2D[] { new(4, 0), new(15, 30) }, 2),
                    "3" => (new Vector2D[] { new(4, 0), new(15, 30) }, 3),
                    _ => (new Vector2D[] { new(4, 0), new(4, 0) }, (int?)null)
                };

                validInput = difficulty != null;
            } while (!validInput);

            return (arenaSize, (int)difficulty);
        }

        /// <summary>
        /// Exit the game. Kill the process
        /// </summary>
        private static void ExitGame() => Environment.Exit(0);
        
        /// <summary>
        /// :)
        /// </summary>
        private static void RollCredits()
        {
            Console.Clear();
            Console.WriteLine("Written by yours truly");
            Console.WriteLine("......................");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
